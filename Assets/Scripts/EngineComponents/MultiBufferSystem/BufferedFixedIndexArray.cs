using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UpdateManager;

class BufferedFixedIndexArray<T>
{
    FixedIndexArray<T>[] arrays = new FixedIndexArray<T>[UpdateManager.BufferCount];
    #region get/setters
    public T GetCurrent(int index)
    {
        return arrays[CurrentBuffer].Get(index);
    }
    public T GetLast(int index)
    {
        return arrays[LastBuffer].Get(index);
    }
    public void CopyLast()
    {
        Last.CopyTo(Current);
    }
    public FixedIndexArray<T> Current { get => arrays[CurrentBuffer]; }
    public FixedIndexArray<T> Last { get => arrays[LastBuffer]; }
    #endregion
    public ulong createdTime { get; private set; }
    public BufferedFixedIndexArray()
    {
        createdTime = UpdateManager.Instance.frameNo;
        for (int i = 0; i < BufferCount; i++)
        {
            arrays[i] = new FixedIndexArray<T>();
        }
    }
}
