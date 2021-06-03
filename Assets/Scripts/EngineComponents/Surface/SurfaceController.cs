using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SurfaceController
{
    ulong CreatedAt;
    public  //should delet
        SurfaceChunkController chunkController;
    BufferedFixedIndexArray<SurfaceObject> surfaceObjects;
    SurfaceGen surfaceGen;

    public SurfaceController(byte[] seed)
    {
        surfaceGen = new SurfaceGen(seed);
        CreatedAt = UpdateManager.FrameNo;
        surfaceObjects = new BufferedFixedIndexArray<SurfaceObject>();
        chunkController = new SurfaceChunkController(surfaceObjects);
    }

    public void RegisterEvents(List<SurfaceEvent> events)
    {
        Debug.Assert(events.TrueForAll(a => a.RegistedFrame == UpdateManager.FrameNo || a.RegistedFrame == null));
        foreach (var ev in events)
        {
            if (ev is SurfacePlaceObjectEvent)
            {
                SurfacePlaceObjectEvent spoe = ev as SurfacePlaceObjectEvent;


                chunkController.CanPlaceObject(spoe.position, spoe.blockType);

                SurfaceObject newObject = new SurfaceObject(spoe.position, spoe.blockType);
                int newObjectIndex = surfaceObjects.Current.Add(newObject);
                chunkController.RegisterObject(newObjectIndex);

                //SurfaceEvent
            }
            if (ev is SurfaceGenerateMapEvent)
            {
                SurfaceGenerateMapEvent sgme = ev as SurfaceGenerateMapEvent;

                Debug.Assert(chunkController.ChunkExists(sgme.chunkNo.x, sgme.chunkNo.y) == false);

                int fromx = SurfaceChunkController.chunkSize * sgme.chunkNo.x;
                int tox = Mathf.Min(SurfaceChunkController.chunkSize * (sgme.chunkNo.x + 1), chunkController.mapWidth);

                int fromy = SurfaceChunkController.chunkSize * sgme.chunkNo.y;
                int toy = SurfaceChunkController.chunkSize * (sgme.chunkNo.y + 1);
                
                for (int x = fromx; x < tox; x++)
                {
                    for (int y = fromy; y < toy; y++)
                    {
                        int blockNo = surfaceGen.SurfaceGenV0(x, y);
                        if (blockNo != 0)
                        {
                            var sObject = new SurfaceObject(new Unity.Mathematics.int2(x, y), blockNo);
                            int sObjIdx = surfaceObjects.Current.Add(sObject);
                            chunkController.RegisterObject(sObjIdx);
                        }
                        
                    }
                }
            }
        }
    }

    public void PrepareNextFrame()
    {
        surfaceObjects.CopyToNext();
        chunkController.PrepareNextFrame();
    }
    public JobHandle DoUpdate()
    {
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

        throw new System.NotImplementedException();
    }
}
