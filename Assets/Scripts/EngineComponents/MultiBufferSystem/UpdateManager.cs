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
    public const int BufferCount = 3;
    public static int CurrentBuffer; //{ get; private set; }
    public static int LastBuffer;// { get; private set; }//todo re-enable for release
    public static byte frameHash;// { get; private set; }
    public static ulong FrameNo { get; private set; } = 0;
    public static ulong UpdatingFrame => FrameNo;
    public static ulong LastFrame => FrameNo - 1;
    public static ulong NextFrame => FrameNo + 1;

    void Init()
    {
        LastBuffer = 0;
        CurrentBuffer = 1;
    }

    Task PrevFrameTask = null;
    // Update is called once per frame
    void Update()
    {
        PrevFrameTask?.Wait();

        ++FrameNo;
        frameHash = (byte)(FrameNo % byte.MaxValue);
        LastBuffer = CurrentBuffer;
        CurrentBuffer = (CurrentBuffer + 1) % BufferCount;

        var prepair = SurfaceManager.Instance.PrepareFrame();

        SurfaceManager.Instance.DoInput();

        prepair.Wait();

        PrevFrameTask = SurfaceManager.Instance.DoUpdate();
    }
}
