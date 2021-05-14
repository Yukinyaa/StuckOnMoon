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
    public const int BufferCount = 4;
    public static int CurrentBuffer; //{ get; private set; }
    public static int LastBuffer;// { get; private set; }//todo re-enable for release
    public static byte FrameHash;// { get; private set; }
    public static int? LockedFrame = null;
    public static int NextBuffer
    {
        get => (CurrentBuffer + 1) % BufferCount == LockedFrame ?
              (CurrentBuffer + 2) % BufferCount :
              (CurrentBuffer + 1) % BufferCount;
    }
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
        InputManager.Instance.DoInput();

        PrevFrameTask?.Wait();

        ++FrameNo;
        FrameHash = (byte)(FrameNo % byte.MaxValue);
        LastBuffer = CurrentBuffer;

        do
        {
            CurrentBuffer = (CurrentBuffer + 1) % BufferCount;
        } while (LockedFrame == CurrentBuffer);


        SurfaceManager.Instance.ProcessEvents();

        PrevFrameTask = SurfaceManager.Instance.DoUpdate();

        SurfaceManager.PrepairRender();


    }
}
