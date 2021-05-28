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
    BufferedFixedIndexArray<SurfaceObject> surfaceObejcts;
    
    List<SurfaceEvent> sEvent;
    public SurfaceController()
    {
        CreatedAt = UpdateManager.FrameNo;
        surfaceObejcts = new BufferedFixedIndexArray<SurfaceObject>();
        chunkController = new SurfaceChunkController(surfaceObejcts);
    }

    public void RegisterEvents(List<SurfaceEvent> events)
    {
        Debug.Assert(events.TrueForAll(a => a.RegistedFrame == UpdateManager.FrameNo || a.RegistedFrame == null));
        sEvent = events;
        foreach (var ev in events)
        {
            if (ev is SurfacePlaceObjectEvent)
            {
                SurfacePlaceObjectEvent spoe = ev as SurfacePlaceObjectEvent;


                chunkController.CanPlaceObject(spoe.position, spoe.blockType);

                SurfaceObject newObject = new SurfaceObject(spoe.position, spoe.blockType);
                int newObjectIndex = surfaceObejcts.Current.Add(newObject);
                chunkController.RegisterObject(newObjectIndex);

                //SurfaceEvent
            }
            if (ev is SurfaceGenerateMapEvent)
            { 
                
            }
        }
    }

    public void PrepareNextFrame()
    {
        surfaceObejcts.CopyToNext();
        chunkController.PrepareNextFrame();
    }
    public void DoUpdate()
    {
        
    }
    //JobHandle DoUpdate()
    //{
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

    //    throw new System.NotImplementedException();
    //}
}
