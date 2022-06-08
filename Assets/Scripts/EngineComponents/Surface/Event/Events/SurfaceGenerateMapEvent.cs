using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
public class SurfaceGenerateMapEvent : SurfaceEvent
{
    public int2 chunkNo { get; protected set; }//minX basically
    public bool forceGenerate { get; protected set; }
    public SurfaceGenerateMapEvent(int surfaceNo, int2 chunkNo, bool forceGenerate, int? issuedUID = null, ulong? registedFrame = null)
        : base(surfaceNo, issuedUID, registedFrame)
    {
        this.chunkNo = chunkNo;
        this.forceGenerate = forceGenerate;
    }
}