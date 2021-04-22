using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SurfaceController
{ 
    
    public  //should delet
        ChunkManager chunkController = new ChunkManager();
    BufferedFixedIndexArray<SurfaceObject> surfaceObejcts = new BufferedFixedIndexArray<SurfaceObject>();
    
    List<SurfaceEvent> sEvent;
    public void PrepareFrame()
    {
        chunkController.PrepareFrame();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
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
