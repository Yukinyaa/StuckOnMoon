using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoM.SurfaceGen
{
    class SurfaceGenPerlinNoise : SurfaceGenAbstractNode<Fix64>
    {
        public Fix64[] seed;
        public int xOffset, yOffset, zOffset;//offsets
        public int scale;
        public Fix64 clampValue;
        public override Fix64 GetValueAt(int x, int y)
        {
            return Fix64.perlin(
                (Fix64)xOffset + (Fix64)x / (Fix64)scale,
                (Fix64)yOffset + (Fix64)y / (Fix64)scale,
                (Fix64)zOffset);
        }
    }
}

