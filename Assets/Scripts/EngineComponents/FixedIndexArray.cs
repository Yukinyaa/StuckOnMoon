using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

struct FixedArrayElement<T> {
    public T element;
    public int nextFreeIndex;
}
public class FixedIndexArray<T> : IEnumerable<T>
{

    int firstFreeIndex = -1;// first free index, fifo
    int lastFreeIndex = 0;  // last free index, only used on RemovAt(idx)
    int count = 0;          // count basically
    FixedArrayElement<T>[] array;

    public T this[int index] {
        get => (array[index].nextFreeIndex == -1) ? 
            array[index].element : throw new IndexOutOfRangeException("deleted component");
        
        set { 
            if (array[index].nextFreeIndex == -1) 
                array[index].element = value; 
            else 
                throw new IndexOutOfRangeException("deleted component");
        }
    }
    public T SafeGet(int index)
    { 
    return (array[index].nextFreeIndex == -1) ?
            array[index].element : throw new IndexOutOfRangeException("deleted component");
    }
    public void SafeSet(int index, T value)
    {
        if (array[index].nextFreeIndex == -1)
            array[index].element = value;
        else
            throw new IndexOutOfRangeException("deleted component");
    }
    public T Get(int index)
    {
        return array[index].element;
    }
    public void Set(int index, T value)
    {
        array[index].element = value;
    }
    //tip: use blockcopy to add performance to copy for 
    public int Count => count;

    public int Add(T item)
    {
        ++count;
        if (array.Length >= count)
        {
            FixedArrayElement<T>[] newArray = new FixedArrayElement<T>[array.Length * 2];
            Buffer.BlockCopy(array, 0, newArray, 0, array.Length);
            array = newArray;
        }
        
        if (firstFreeIndex == -1)
        {
            array[count - 1] = new FixedArrayElement<T> { element = item, nextFreeIndex = -1 };
            return count - 1;
        }
        else
        {
            int insertTo = firstFreeIndex;
            firstFreeIndex = array[insertTo].nextFreeIndex;
            array[insertTo].nextFreeIndex = -1;
            array[insertTo].element = item;
            return insertTo;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        --count;
        if (firstFreeIndex == -1)
        {
            firstFreeIndex = index;
            lastFreeIndex = index;
        }
        else 
        {
            array[lastFreeIndex].nextFreeIndex = index;
            lastFreeIndex = index;
        }
        //don't remove remaining data basically :), used union so will be damaged somehow

    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
