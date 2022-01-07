using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public struct FixedArrayElement<T> {
    public T element;
    public int nextFreeIndex;
    public override string ToString()
    {
        return $"{nextFreeIndex}* : {element.ToString()}";
    }
}

public delegate void ModAction<T>(ref T target);


//todo: serialize, deserialize
public class FixedIndexArray<T> : IEnumerable<T>
{

    int firstFreeIndex = -1;// first free index, fifo
    int lastFreeIndex = 0;  // last free index, only used on RemovAt(idx)
    int count = 0;          // count basically
    int maxIndex = -1;
    FixedArrayElement<T>[] array = new FixedArrayElement<T>[8];


    #region get/setters
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

    public bool SafeGet(int index, out T result)
    {
        result = default;
        if (maxIndex < index)
            return false;

        if (array[index].nextFreeIndex != -1)
            return false;
        
        result = array[index].element;
        return true;
    }

    internal void Remove(int objID)
    {
        throw new NotImplementedException();
    }

    public void SafeSet(int index, in T value)
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
    public void Set(int index, in T value)
    {
        array[index].element = value;
    }
    #endregion



    public int Count { get => count; }
    public int MaxIndex { get => maxIndex; }
    public int Length { get => array.Length; }

    //tip: use blockcopy to add performance to copy for => don't have to I guess
    public int Add(T item)
    {
        ++count;
        if (array.Length <= count)
        {
            FixedArrayElement<T>[] newArray = new FixedArrayElement<T>[array.Length * 2];
            array.CopyTo(newArray, 0);
            //Buffer.BlockCopy(array, 0, newArray, 0, array.Length);
            array = newArray;
        }
        
        if (firstFreeIndex == -1)
        {
            array[count - 1] = new FixedArrayElement<T> { element = item, nextFreeIndex = -1 };
            maxIndex++;
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
    public void CopyTo(FixedIndexArray<T> other)
    {
        other.CopyFrom(firstFreeIndex, lastFreeIndex, count, maxIndex, array);
    }
    public void CopyFrom(int firstFreeIndex, int lastFreeIndex, int count, int maxIndex, FixedArrayElement<T>[] array_)
    {
        this.firstFreeIndex = firstFreeIndex;
        this.lastFreeIndex = lastFreeIndex;
        this.count = count;
        this.maxIndex = maxIndex;
        if (array_.Length > this.array.Length)
            this.array = new FixedArrayElement<T>[array_.Length];
        array_.CopyTo(this.array, 0);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    public Enumerator GetCustomEnumerator()
    {
        return new Enumerator(this);
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


    public void ForEach(ModAction<T> action)
    {
        T thisIdx;
        for (int i = 0; i <= maxIndex; i++)
        {
            if (SafeGet(i, out thisIdx))
                action(ref array[i].element);
        }
    }
    public void ForEach(Action<T, int> action)
    {
        T thisIdx;
        for (int i = 0; i <= maxIndex; i++)
        {
            if (SafeGet(i, out thisIdx))
                action(thisIdx, i);
        }
    }
    public void ForEach(Action<T> action)
    {
        T thisIdx;
        for (int i = 0; i <= maxIndex; i++)
        {
            if(SafeGet(i, out thisIdx))
                action(thisIdx);
        }
    }


    public System.Collections.Generic.IEnumerable<T> Iterator()
    {
        T thisIdx;
        for (int i = 0; i <= maxIndex; i++)
        {
            if (SafeGet(i, out thisIdx))
                yield return (thisIdx);
        }
    }

    public System.Collections.Generic.IEnumerable<(T, int)> IteratorWithIndex()
    {
        T thisIdx;
        for (int i = 0; i <= maxIndex; i++)
        {
            if (SafeGet(i, out thisIdx))
                yield return (thisIdx, i);
        }
    }

    public bool Exists(Predicate<T> action)
    {
        T thisIdx;
        for (int i = 0; i <= maxIndex; i++)
        {
            if (SafeGet(i, out thisIdx))
                if (action(thisIdx))
                    return true;
        }
        return false;
    }


    public class Enumerator : IEnumerator<T>
    {
        
        FixedIndexArray<T> target;

        public Enumerator(FixedIndexArray<T> target) {
            this.target = target;
        }

        public int Index { get; private set; } = -1;

        T IEnumerator<T>.Current => target.Get(Index);

        public object Current => target.Get(Index);

        
        public bool MoveNext()
        {
            while (!target.SafeGet(++Index, out _))
                if (Index > target.maxIndex)
                    return false;
            return true;
        }

        public void Reset()
        {
            Index = 0;
        }
        void IDisposable.Dispose()
        {
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }
}
