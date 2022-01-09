using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class SurfaceController
{
    ulong CreatedAt;
    SurfaceObejctsController sObjectsController;
    SurfaceGen surfaceGen;
    SurfaceRenderer renderer;
    int surfaceNo;
    public int2 gridOffset;

    public int MapWidth { get => sObjectsController.GetSurfaceInfo.width; }

    public string Name { get; private set; } = "nauvis";
    SurfaceInfo surfaceInfo;
    public SurfaceController(byte[] seed, int surfaceNo)
    {
        surfaceGen = new SurfaceGen(seed);
        CreatedAt = UpdateManager.UpdatingFrameNo;
        this.surfaceNo = surfaceNo;

        surfaceInfo.surfaceNo = surfaceNo;
        surfaceInfo.width = 4200;
        surfaceInfo.height = 1200;

        sObjectsController = new SurfaceObejctsController(surfaceInfo);

        var go = new GameObject($"{Name} Renderer");
        renderer = go.AddComponent<SurfaceRenderer>();

        renderer.Init(SurfaceChunkController.XChunkCountFromSurfaceInfo(surfaceInfo),
                        SurfaceChunkController.YChunkCountFromSurfaceInfo(surfaceInfo));
    }



    List<SurfaceEvent> eventsToProcess;
    public void RegisterEvents(List<SurfaceEvent> events)
    {
        this.eventsToProcess = events;
    }
    public void ProcessEvents()
    {
        Debug.Assert(eventsToProcess.TrueForAll(a => a.RegistedFrame == UpdateManager.UpdatingFrameNo || a.RegistedFrame == null));
        foreach (var ev in eventsToProcess)
        {
            if (ev is SurfacePlaceObjectEvent)
            {
                SurfacePlaceObjectEvent spoe = ev as SurfacePlaceObjectEvent;
                bool success = sObjectsController.TryPlaceObject(spoe.position, spoe.blockType,out int newObjectIndex);

                //SurfaceEvent
            }
            if (ev is SurfaceGenerateMapEvent)
            {
                SurfaceGenerateMapEvent sgme = ev as SurfaceGenerateMapEvent;

                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                bool success = GenerateMap(sgme.chunkNo);
                stopwatch.Stop();
                if (success)
                    Debug.Log($"Generated Map {sgme.chunkNo.x}, {sgme.chunkNo.y}: {stopwatch.Elapsed.ToString()}");
                else
                    Debug.Log($"{sgme.chunkNo.x}, {sgme.chunkNo.y}: {stopwatch.Elapsed.ToString()}");
            }
        }
    }

    private bool GenerateMap(int2 chunkNo)
    {
        if (sObjectsController.IsChunkGenerated(chunkNo.x, chunkNo.y))
            return false;
        //Debug.Log($"Generating Map {chunkNo.x}, {chunkNo.y}");

        Debug.Assert(sObjectsController.IsChunkGenerated(chunkNo.x, chunkNo.y) == false);

        int fromx = SurfaceChunkController.chunkSize * chunkNo.x;
        int tox = Mathf.Min(SurfaceChunkController.chunkSize * (chunkNo.x + 1), surfaceInfo.width);

        int fromy = SurfaceChunkController.chunkSize * chunkNo.y;
        int toy = SurfaceChunkController.chunkSize * (chunkNo.y + 1);
        sObjectsController.MarkMapGenerated(chunkNo.x, chunkNo.y);


        int blkcnt = 0;
        for (int x = fromx; x < tox; x++)
        {
            for (int y = fromy; y < toy; y++)
            {
                int blockNo = surfaceGen.SurfaceGenV0(x, y);
                if (blockNo != 0)
                {
                    var success = sObjectsController.TryPlaceObject(new int2(x, y), blockNo, out _);
                    if (success)
                        blkcnt++;
                }

            }
        }
        Debug.Log($"Generated Map {chunkNo.x}, {chunkNo.y} (blkcnt : {blkcnt})");
        return true;
    }

    public void PrepareNextFrame()
    {
        sObjectsController.PrepareNextFrame();
    }
    public void DoRender()
    {
        int2 rangeMin, rangeMax;
        GetScreenWorldResolutionWithMargin(out rangeMin, out rangeMax);

        sObjectsController.GenerateUnknownChunkChunkInRange(rangeMin, rangeMax);
        var renderObj = sObjectsController.GetRenderingObjectsInChunkRangeItor(rangeMin, rangeMax);
        var renderBlk = sObjectsController.GetRenderingBlocksInChunkRangeItor(rangeMin, rangeMax);
        renderer.DoRender(renderObj, renderBlk);
    }


    public Vector2 GridPositionAsWorldPosition(int2 gridPos)
    {
        return new Vector2(gridPos.x + gridOffset.x, gridPos.y + gridOffset.y);
    }

    public int2 WorldPositionAsGridPosition(Vector2 worldPos)
    {
        return new int2(Mathf.FloorToInt((worldPos.x - gridOffset.x)), Mathf.FloorToInt(worldPos.y - gridOffset.y));
    }
    public int2 MousePositionAsGridPosition()
    {
        Vector3 input = Input.mousePosition;
        input.z = 0;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(input);  //tip: rendering camera may not be main
        return new int2(Mathf.FloorToInt(worldPos.x - gridOffset.x), Mathf.FloorToInt(worldPos.y - gridOffset.y));
    }

    private void GetScreenWorldResolutionWithMargin(out int2 rangeMin, out int2 rangeMax)
    {
        float chunkPreloadMarginRatio = 0.1f;

        Vector2 resolution2 = new Vector2(Screen.width, Screen.height);

        Vector2 camMinWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(resolution2.x * (-chunkPreloadMarginRatio), resolution2.y * (-chunkPreloadMarginRatio), 0));
        Vector2 camMaxWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(resolution2.x * (1 + chunkPreloadMarginRatio), resolution2.y * (1 + chunkPreloadMarginRatio), 0));

        rangeMin = WorldPositionAsGridPosition(camMinWorldPoint);
        rangeMax = WorldPositionAsGridPosition(camMaxWorldPoint);
    }

    public void DoUpdate()
    {
        ProcessEvents();




        /* network+user+extra events happens here idk */

        // what if electricity shit gets destoryed here?? >> then change sum

        /* consume/produce products */
        /* aka update factories. FactoryUpadateStat? idk */
        // set isActive too to calculate next frame

        // use isChanged on fluid systems to flag unchanged systems?


        //if done, calculate logistic movements
        /*
        NativeArray<JobHandle> prepairJobs =
            new NativeArray<JobHandle>(4,
                SurfaceFluidManager.RunPrepairJob(), //simply copy amounts
                SurfaceGasManager.RunPrepairJob(),   //simply copy amounts 
                SurfaceSTransportManager.RunPrepairJob(), //simply copy amounts
                SurfaceElectricityManager.RunPrepairJob() //sum(active producers) / sum(active consumers)
            );

        //*/
    }
}
