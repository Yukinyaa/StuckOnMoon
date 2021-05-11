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
    
    public T GetCurrent(int index)
    {
        return arrays[CurrentBuffer].Get(index);
    }
    public bool SafeGetCurrent(int index, out T data)
    {
        return arrays[CurrentBuffer].SafeGet(index, out data);
    }

    public T GetLast(int index)
    {
        return arrays[LastBuffer].Get(index);
    }

    public bool SafeGetLast(int index, out T data)
    {
        return arrays[LastBuffer].SafeGet(index, out data);
    }


    public void CopyLast()
    {
        Last.CopyTo(Current);
    }
    public void CopyToNext()
    {
        Current.CopyTo(Next);
    }
    public FixedIndexArray<T> Current { get => arrays[CurrentBuffer]; }
    public FixedIndexArray<T> Last { get => arrays[LastBuffer]; }

    public FixedIndexArray<T> Next { get => arrays[NextBuffer]; }
    #endregion
    public ulong createdTime { get; private set; }
    public BufferedFixedIndexArray()
    {
        createdTime = UpdateManager.FrameNo;
        for (int i = 0; i < BufferCount; i++)
        {
            arrays[i] = new FixedIndexArray<T>();
        }
    }
}
