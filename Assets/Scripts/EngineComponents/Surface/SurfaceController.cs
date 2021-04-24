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
    public void RegisterEvents(List<SurfaceEvent> @event)
    {
        Debug.Assert(@event.TrueForAll(a => a.RegistedFrame == UpdateManager.frameNo));
            sEvent = @event;
    }
    public SurfaceController()
    {
        CreatedAt = UpdateManager.frameNo;
        surfaceObejcts = new BufferedFixedIndexArray<SurfaceObject>();
        chunkController = new SurfaceChunkController(surfaceObejcts);
    }
    public void PrepareFrame()
    {
        chunkController.PrepareFrame();
    }
    public void DoRender()
    { 
        
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
