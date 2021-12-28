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
    public void UpdateAllSurfaces()
    {
        var tasks = from SurfaceController s in surfaces select Task.Factory.StartNew(() => { s.DoUpdate(); s.PrepareNextFrame(); });
        Task.WaitAll(tasks.ToArray());
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

    public void RegisterEventsToSurface()
    {
        var events = EventManager.Instance.PopEvents(UpdateManager.UpdatingFrameNo);
        

        //process global event ex)chatting etc

        for (int i = 0; i <= surfaces.MaxIndex; i++)
        {
            surfaces.SafeGet(i, out var surface);
            surface.RegisterEvents(events.Where(a => a.SurfaceNo == i).ToList());
        }
    }
    // Update is called once per frame
    public Task DoUpdate()
    {
        return Task.Factory.StartNew(() =>
        {
            UnityEngine.Profiling.Profiler.BeginThreadProfiling("Custom Update Threads", "Surfaces Update");

            UnityEngine.Profiling.Profiler.BeginSample("UpdateAllSurfaces");
            UpdateAllSurfaces();
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
}
