using System;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class UpdateManager : Singleton<UpdateManager>
{

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    public const int BufferCount = 5;
    public static int RenderedBuffer { get; private set; }
    public static int UpdatingBuffer { get; private set; }
    public static int RenderingBuffer { get; private set; }
    public static byte FrameHash { get; private set; }
    public static int? LockedFrame = null;
    public static int NextBuffer
    {
        get => (UpdatingBuffer + 1) % BufferCount == LockedFrame ?
              (UpdatingBuffer + 2) % BufferCount :
              (UpdatingBuffer + 1) % BufferCount;
    }
    public static ulong UpdatingFrameNo { get; private set; } = 0;
    public static ulong UpdatingFrame => UpdatingFrameNo;
    public static ulong RenderingFrame => UpdatingFrameNo - 1;
    public static ulong NextUpdatingFrame => UpdatingFrameNo + 1;

    void Init()
    {
        RenderedBuffer = 0;
        RenderingBuffer = 1;
        UpdatingBuffer = 2;
    }

    Task PrevFrameTask = null;
    // Update is called once per frame
    public void Update()
    {
        //InputManager.Instance.DoInput();
        UnityEngine.Profiling.Profiler.BeginSample("Waiting Update");
        PrevFrameTask?.Wait();
        UnityEngine.Profiling.Profiler.EndSample();


        ++UpdatingFrameNo;
        FrameHash = (byte)(UpdatingFrameNo % byte.MaxValue);
        RenderedBuffer = RenderingBuffer;
        RenderingBuffer = UpdatingBuffer;
        do
        {
            UpdatingBuffer = (UpdatingBuffer + 1) % BufferCount;
        } while (LockedFrame == UpdatingBuffer);
        // if saving is finished unlock buffer here

        // Debug.Log($"Buff state: {RenderedBuffer}, {RenderingBuffer}, {UpdatingBuffer}, {NextBuffer}");
        SurfaceManager.Instance.RegisterEventsToSurface();

        PrevFrameTask = SurfaceManager.Instance.DoUpdate();

        SurfaceManager.Instance.Render();
    }
}
