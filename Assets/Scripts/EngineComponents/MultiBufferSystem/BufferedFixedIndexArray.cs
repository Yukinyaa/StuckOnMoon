using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UpdateManager;

public class BufferedFixedIndexArray<T>
{
    FixedIndexArray<T>[] arrays = new FixedIndexArray<T>[UpdateManager.BufferCount];
    #region get/setters
    
    public T GetUpdating(int index)
    {
        return arrays[UpdatingBuffer].Get(index);
    }
    public bool SafeGetUpdating(int index, out T data)
    {
        return arrays[UpdatingBuffer].SafeGet(index, out data);
    }

    public T GetRendering(int index)
    {
        return arrays[RenderingBuffer].Get(index);
    }

    public bool SafeGetRendering(int index, out T data)
    {
        return arrays[RenderingBuffer].SafeGet(index, out data);
    }


    public void CopyFromRendering()
    {
        Rendering.CopyTo(Updating);
    }
    public void CopyUpdateToNext()
    {
        Updating.CopyTo(NextUpdate);
    }
    public FixedIndexArray<T> Updating { get => arrays[UpdatingBuffer]; }
    public FixedIndexArray<T> Rendering { get => arrays[RenderingBuffer]; }

    public FixedIndexArray<T> NextUpdate { get => arrays[NextBuffer]; }
    #endregion
    public ulong createdTime { get; private set; }
    public BufferedFixedIndexArray()
    {
        createdTime = UpdateManager.UpdatingFrameNo;
        for (int i = 0; i < BufferCount; i++)
        {
            arrays[i] = new FixedIndexArray<T>();
        }
    }
}
