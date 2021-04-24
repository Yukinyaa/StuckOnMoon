public class SurfaceEvent
{
    public SurfaceEvent(int surfaceNo,  ulong? registedFrame = null, int? issuedUID = null)
    {
        
        this.RegistedFrame = registedFrame ?? UpdateManager.frameNo;
        this.IssuedUID = issuedUID ?? 0;//todo : multiplayerManager.myUID or sth like that
        this.SurfaceNo = surfaceNo;
    }
    //


    public ulong RegistedFrame { get; private set; }
    public int IssuedUID { get; protected set; }
    public int SurfaceNo { get; protected set; }
}