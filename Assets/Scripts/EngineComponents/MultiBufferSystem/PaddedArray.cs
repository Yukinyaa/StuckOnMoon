using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class PaddedRefBuffer<T> where T : class
{
    [StructLayout(LayoutKind.Auto, Size = 64)]
    struct PaddedRType
    {
        public T elem;
    }

    PaddedRType[] paddedBuffer;

    public PaddedRefBuffer()
    { 
        paddedBuffer = new PaddedRType[UpdateManager.BufferCount];
    }


    public T this[int idx]
    {
        get => paddedBuffer[idx].elem;
        set => paddedBuffer[idx].elem = value;
    }
    public T Rendering => paddedBuffer[UpdateManager.RenderedBuffer].elem;
    public T Updating => paddedBuffer[UpdateManager.UpdatingBuffer].elem;
    public T Next => paddedBuffer[UpdateManager.NextBuffer].elem;
}