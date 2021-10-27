using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class SurfaceController
{
    ulong CreatedAt;
    public  //should delet
        SurfaceChunkController chunkController;
    BufferedFixedIndexArray<SurfaceObject> surfaceObjects;
    SurfaceGen surfaceGen;
    SurfaceRenderer renderer;
    int surfaceNo;
    public int2 gridOffset;


    public string Name { get; private set; } = "nauvis";

    public SurfaceController(byte[] seed, int surfaceNo)
    {
        surfaceGen = new SurfaceGen(seed);
        CreatedAt = UpdateManager.UpdatingFrameNo;
        surfaceObjects = new BufferedFixedIndexArray<SurfaceObject>();
        this.surfaceNo = surfaceNo;
        
        chunkController = new SurfaceChunkController(surfaceObjects, surfaceNo);

        var go = new GameObject($"{Name} Renderer");
        renderer = go.AddComponent<SurfaceRenderer>();


        renderer.Init(chunkController.countX, chunkController.countY);
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


                chunkController.CanPlaceObject(spoe.position, spoe.blockType);

                SurfaceObject newObject = new SurfaceObject(spoe.position, spoe.blockType);
                int newObjectIndex = surfaceObjects.Updating.Add(newObject);
                chunkController.RegisterObject(newObjectIndex);

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
                    Debug.Log($"Generatied Map {sgme.chunkNo.x}, {sgme.chunkNo.y}: {stopwatch.Elapsed.ToString()}");


            }
        }
    }

    private bool GenerateMap(int2 chunkNo)
    {
        if (chunkController.ChunkExists(chunkNo.x, chunkNo.y))
            return false;
        //Debug.Log($"Generating Map {chunkNo.x}, {chunkNo.y}");

        Debug.Assert(chunkController.ChunkExists(chunkNo.x, chunkNo.y) == false);

        int fromx = SurfaceChunkController.chunkSize * chunkNo.x;
        int tox = Mathf.Min(SurfaceChunkController.chunkSize * (chunkNo.x + 1), chunkController.mapWidth);

        int fromy = SurfaceChunkController.chunkSize * chunkNo.y;
        int toy = SurfaceChunkController.chunkSize * (chunkNo.y + 1);
        chunkController.Generate(chunkNo.x, chunkNo.y);


        int blkcnt = 0;
        for (int x = fromx; x < tox; x++)
        {
            for (int y = fromy; y < toy; y++)
            {
                int blockNo = surfaceGen.SurfaceGenV0(x, y);
                if (blockNo != 0)
                {
                    var sObject = new SurfaceObject(new int2(x, y), blockNo);
                    int sObjIdx = surfaceObjects.Updating.Add(sObject);
                    chunkController.RegisterObject(sObjIdx);
                    blkcnt++;
                }

            }
        }
        Debug.Log($"Generated Map {chunkNo.x}, {chunkNo.y} (blkcnt : {blkcnt})");
        return true;
    }

    public void PrepareNextFrame()
    {
        surfaceObjects.CopyUpdateToNext();
        chunkController.PrepareNextFrame();
    }
    public void DoRender()
    {
        int2 rangeMin, rangeMax;
        {
            float chunkPreloadMargin = 0.1f;
            
            var resolution = Screen.currentResolution;
            Vector2 resolution2 = new Vector2(resolution.width, resolution.height);

            Vector2 camMinWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(resolution2.x * (  - chunkPreloadMargin), resolution2.y * (  - chunkPreloadMargin), 0));
            Vector2 camMaxWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(resolution2.x * (1 + chunkPreloadMargin), resolution2.y * (1 + chunkPreloadMargin), 0));

            rangeMin = WorldPositionAsGridPosition(camMinWorldPoint);
            rangeMax = WorldPositionAsGridPosition(camMaxWorldPoint);
        }

        chunkController.UnknownChunkChunkInRange(rangeMin, rangeMax);
        renderer.StartObjectUpdate();
        chunkController.ForEachLastObjectsInChunkRange(rangeMin, rangeMax, (obj, index) => renderer.UpdateObject(obj, index));
        renderer.FinishObjectUpdate();
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
    public void DoUpdate()
    {
        ProcessEvents();
        /*
        NativeArray<JobHandle> prepairJobs =
            new NativeArray<JobHandle>(4,
                SurfaceFluidManager.RunPrepairJob(), //simply copy amounts
                SurfaceGasManager.RunPrepairJob(),   //simply copy amounts 
                SurfaceSTransportManager.RunPrepairJob(), //simply copy amounts
                SurfaceElectricityManager.RunPrepairJob() //sum(active producers) / sum(active consumers)
            );

        //*/



        /* network+user+extra events happens here idk */

        // what if electricity shit gets destoryed here?? >> then change sum

        /* consume/produce products */
        /* aka update factories. FactoryUpadateStat? idk */
        // set isActive too to calculate next frame

        // use isChanged on fluid systems to flag unchanged systems?


        //if done, calculate logistic movements

        //throw new System.NotImplementedException();
    }
}
