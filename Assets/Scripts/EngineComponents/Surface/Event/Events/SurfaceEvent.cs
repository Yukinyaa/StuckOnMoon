using System.Collections.Generic;

public class SurfaceEventComparerByFrame : IComparer<SurfaceEvent>
{
    int IComparer<SurfaceEvent>.Compare(SurfaceEvent x, SurfaceEvent y)
    {
        return (int)((x.RegistedFrame ?? 0) - (y.RegistedFrame ?? 0));
    }
}

public class SurfaceEvent
{
    public SurfaceEvent(int surfaceNo,  int? issuedUID = null, ulong? registedFrame = null )
    {
        this.RegistedFrame = registedFrame;
        this.IssuedUID = issuedUID ?? 0;//todo : multiplayerManager.myUID or sth like that
        this.SurfaceNo = surfaceNo;
    }
    //


    public ulong? RegistedFrame { get; private set; }
    public int IssuedUID { get; protected set; }
    public int SurfaceNo { get; protected set; }
}