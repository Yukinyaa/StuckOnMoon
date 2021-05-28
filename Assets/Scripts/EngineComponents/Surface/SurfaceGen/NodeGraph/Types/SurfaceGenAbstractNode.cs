using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SoM.SurfaceGen
{
    public abstract class SurfaceGenAbstractNode<T>
    {
        public abstract T GetValueAt(int x, int y);
    }
}

