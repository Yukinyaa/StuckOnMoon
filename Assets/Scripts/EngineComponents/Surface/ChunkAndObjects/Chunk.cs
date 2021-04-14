using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Chunk
{
    Chunk[] neghbors = new Chunk[8];// +1,1, +1,0, +1,-1, 0,-1, -1,-1
    List<SurfaceObject> objects;
}
