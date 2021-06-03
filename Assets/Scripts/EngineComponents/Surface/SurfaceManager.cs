using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using System.Threading.Tasks;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SurfaceManager : Singleton<SurfaceManager>
{


    [SerializeField]
    Vector3 gridOffset = Vector3.zero;
    [SerializeField]
    Vector3 gridSize = Vector3.one;


    FixedIndexArray<SurfaceController> surfaces;// = new SurfaceController();

#if UNITY_EDITOR
    [SerializeField]
    private bool isDrawGridGizmoEnabled;
    [SerializeField]
    private bool isDrawBlockGizmoEnabled;
#endif
    public int CurrentSurfaceNo { get; private set; } = 0;
    public SurfaceController CurrentSurface => surfaces.SafeGet(CurrentSurfaceNo);
    public int? ViewingSurfaceNo { get; private set; } = 0;
    public SurfaceController ViewingSurface => 
        ViewingSurfaceNo == null ? 
            null : surfaces.SafeGet(ViewingSurfaceNo ?? 0);

    public SurfaceRenderer[] SurfaceRenderers;

    protected override void Awake()
    {
        base.Awake();
        {//prepair
            SObjectTypes.Init();//move elsewhere
            surfaces = new FixedIndexArray<SurfaceController>();
            surfaces.Add(new SurfaceController(new byte[] { 123, 45, 67 }));
        }
    }
    public Task PrepareFrame()
    {
        return Task.Factory.StartNew( () =>
        {
            var tasks = from SurfaceController s in surfaces select Task.Factory.StartNew(() => { s.PrepareNextFrame(); });
            Task.WaitAll( tasks.ToArray() );
        });
    }

    public void Render()
    {
        int2 rangeMin, rangeMax;
        {
            float chunkPreloadMargin = 0.1f;
            Vector2 camMinWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(  - chunkPreloadMargin,   - chunkPreloadMargin, 0));
            Vector2 camMaxWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(1 + chunkPreloadMargin, 1 + chunkPreloadMargin, 0));
            

            CurrentSurface.chunkController.ForEachLastObjectsInChunkRange(
                
                )
        }
        
        
        CurrentSurface.chunkController.ForEachLastObjectsInChunkRange(
            );
    }

    public void ProcessEvents()
    {
        var events = EventManager.Instance.PopEvents(UpdateManager.UpdatingFrameNo);


        //process global event ex)chatting etc

        for (int i = 0; i <= surfaces.MaxIndex; i++)
        {
            surfaces.SafeGet(i, out var surface);
            surface.RegisterEvents(events.Where(a => a.SurfaceNo == i).ToList());

            //can multithread idk
        }
    }
    // Update is called once per frame
    public Task DoUpdate()
    {
        return Task.Factory.StartNew(() =>
        {
            UnityEngine.Profiling.Profiler.BeginThreadProfiling("Custom Update Threads", "Surfaces Update");


            UnityEngine.Profiling.Profiler.BeginSample("PrepairNextFrame");
            PrepareFrame().Wait();
            UnityEngine.Profiling.Profiler.EndSample();

            UnityEngine.Profiling.Profiler.EndThreadProfiling();
        });
    }
    
    public Vector2Int MousePositionAsGridPosition()
    {
        Vector3 input = Input.mousePosition;
        input.z = gridOffset.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(input);  //tip: rendering camera may not be main
        return new Vector2Int(Mathf.FloorToInt((worldPos.x - gridOffset.x) / gridSize.x), Mathf.FloorToInt(worldPos.y - gridOffset.y / gridSize.y));

    }

    void OnDrawGizmos()
    {

#if UNITY_EDITOR
        if (isDrawGridGizmoEnabled)
        {
            for (int x = -4; x <= 4; x++)
                for (int y = 0; y <= 8; y++)
                {
                    
                    DebugExtension.DrawPoint(new Vector3(gridOffset.x + gridSize.x * x, gridOffset.y + gridSize.y * y));
                    Handles.Label(new Vector3(gridOffset.x + gridSize.x * (x + 0.5f), gridOffset.y + gridSize.y * (y + 0.5f)), $"{(x<0?x+ surfaces?.SafeGet(CurrentSurfaceNo).chunkController.mapWidth:x)},{y}", GUIStyle.none);
                }
            DebugExtension.DrawArrow(Vector3.zero, new Vector3(gridSize.x, 0), Color.red);
            DebugExtension.DrawArrow(Vector3.zero, new Vector3(0, gridSize.y), Color.green);
        }


        if (isDrawBlockGizmoEnabled)
        {
            //surface1?.chunkController?.ForEachObject(o => Handles.Label(new Vector3(o.postion.x + 0.5f, o.postion.y + 0.5f), SObjectTypes.sObjectTypes[o.objectType].name));// o.objectType.ToString()
            surfaces?.SafeGet(CurrentSurfaceNo).chunkController?.ForEachObject(o => Handles.Label(new Vector3(o.postion.x + 0.5f, o.postion.y + 0.5f), o.ToString()));// o.objectType.ToString()
            surfaces?.SafeGet(CurrentSurfaceNo).chunkController?.ForEachObject(o => DebugExtension.DrawBounds(
                                new Bounds(o.Middle, new Vector2(o.shape.size.x, o.shape.size.y))
                    ));// o.objectType.ToString()
        }
#endif
    }
}
