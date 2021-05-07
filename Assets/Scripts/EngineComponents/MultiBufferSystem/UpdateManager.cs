using System;
using UnityEngine;
using System.Collections;

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

    // Update is called once per frame
    void Update()
    {
        ++FrameNo;
        frameHash = (byte)(FrameNo % byte.MaxValue);
        LastBuffer = CurrentBuffer;
        CurrentBuffer = (CurrentBuffer + 1) % BufferCount;

        SurfaceManager.Instance.DoUpdate();
    }
}
