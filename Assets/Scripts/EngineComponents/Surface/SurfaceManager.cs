using UnityEngine;
using Unity.Mathematics;
using System.Threading.Tasks;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SurfaceManager : Singleton<SurfaceManager>
{
    FixedIndexArray<SurfaceController> surfaces;// = new SurfaceController();

#if UNITY_EDITOR
    [SerializeField]
    private bool isDrawGridGizmoEnabled;
    [SerializeField]
    private bool isDrawBlockGizmoEnabled;
#endif
    public int2 gridOffset = new int2(0, 0);
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
            surfaces.Add(new SurfaceController(new byte[] { 123, 45, 67 }, 0));
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
        for (int i = 0; i <= surfaces.MaxIndex; i++)
        {
            if (surfaces.SafeGet(i, out var surface))
            {
                if (surface == CurrentSurface || surface == ViewingSurface)
                    surface.DoRender();
            }
        }
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

    public Vector2 GridPositionAsWorldPosition(int2 gridPos)
    {
        return new Vector2(gridPos.x + gridOffset.x, gridPos.y + gridOffset.y);
    }
    public int2 WorldPositionAsGridPosition(Vector2 worldPos)
    {
        return new int2(Mathf.FloorToInt(worldPos.x - gridOffset.x), Mathf.FloorToInt(worldPos.y - gridOffset.y));
    }
    public int2 WorldPositionAsGridLocalPosition(Vector2 worldPos)
    {
        return new int2(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y));
    }
    public int2 MousePositionAsGridPosition()
    {
        Vector3 input = Input.mousePosition;
        input.z = 0;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(input);  //tip: rendering camera may not be main
        return new int2(Mathf.FloorToInt(worldPos.x - gridOffset.x), Mathf.FloorToInt(worldPos.y - gridOffset.y));

    }

    void OnDrawGizmos()
    {

#if UNITY_EDITOR
        if (isDrawGridGizmoEnabled)
        {
            for (int x = -4; x <= 4; x++)
                for (int y = 0; y <= 8; y++)
                {
                    
                    DebugExtension.DrawPoint(new Vector3(gridOffset.x + x, gridOffset.y + y));
                    Handles.Label(new Vector3(gridOffset.x + (x + 0.5f), gridOffset.y + (y + 0.5f)), $"{(x<0?x+ surfaces?.SafeGet(CurrentSurfaceNo).chunkController.mapWidth:x)},{y}", GUIStyle.none);
                }
            DebugExtension.DrawArrow(Vector3.zero, new Vector3(1, 0), Color.red);
            DebugExtension.DrawArrow(Vector3.zero, new Vector3(0, 1), Color.green);
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
