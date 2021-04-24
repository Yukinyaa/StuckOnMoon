using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

class SurfacePlaceObjectEvent : SurfaceEvent
{
    public int blockType { get; protected set; }
    public int2 position { get; protected set; }//minX basically


    public SurfacePlaceObjectEvent(int blockType, int2 position, ulong? registedFrame = null, int? issuedUID = null) : base(registedFrame, issuedUID)
    {
        this.blockType = blockType;
        this.position = position;
    }
}
