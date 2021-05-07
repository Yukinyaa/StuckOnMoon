using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

class SurfacePlaceObjectEvent : SurfaceEvent
{
    public int blockType { get; protected set; }
    public int blockState { get; protected set; }
    public int2 position { get; protected set; }//minX basically


    public SurfacePlaceObjectEvent(int surfaceNo, int blockType, int2 position, int? issuedUID = null, ulong? registedFrame = null) 
        : base(surfaceNo, issuedUID, registedFrame)
    {
        this.blockType = blockType;
        this.position = position;
    }
}
