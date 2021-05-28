using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoM.SurfaceGen
{
    class SurfaceGenClamp : SurfaceGenAbstractNode<bool>
    {
        public SurfaceGenAbstractNode<Fix64> prevNode;
        public Fix64 clampValue;
        public override bool GetValueAt(int x, int y)
        {
            return prevNode.GetValueAt(x, y) > clampValue; // if bigger, return true
        }
    }
}

