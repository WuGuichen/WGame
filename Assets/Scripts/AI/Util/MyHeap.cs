public class MyHeap<T> where T : IHeapItem<T>
{
    public int NowLength { get; private set; }
    public int MaxLength { get; private set; }
    public T Top => heap[0];
    public bool IsEmpty => NowLength == 0;
    public bool IsFull => NowLength >= MaxLength - 1;
    private readonly bool isReverse;
    private readonly T[] heap;

    public MyHeap(int maxLength, bool isReverse = false)
    {
        NowLength = 0;
        MaxLength = maxLength;
        heap = new T[MaxLength-1];
        this.isReverse = isReverse;
    }

    public T this[int index] => heap[index];

    public void Push(T value)
    {
        if (NowLength < MaxLength)
        {
            value.HeapIndex = NowLength;
            heap[NowLength] = value;
            Swim(NowLength);
            ++NowLength;
        }
    }

    public void Pop()
    {
        if (NowLength > 0)
        {
            heap[0] = heap[--NowLength];
            heap[0].HeapIndex = 0;
            Sink(0);
        }
    }

    public bool Contains(T value)
    {
        return Equals(heap[value.HeapIndex]);
    }

    public void Clear()
    {
        for (int i = 0; i < NowLength; ++i)
        {
            heap[i].HeapIndex = 0;
        }

        NowLength = 0;
    }

    private void Swap(T a, T b)
    {
        heap[a.HeapIndex] = b;
        heap[b.HeapIndex] = a;
        (b.HeapIndex, a.HeapIndex) = (a.HeapIndex, b.HeapIndex);
    }

    private void Swim(int index)
    {
        int father;
        while (index > 0)
        {
            father = (index - 1) >> 1;
            if (IsBetter(heap[index], heap[father]))
            {
                Swap(heap[father], heap[index]);
                index = father;
            }
            else
            {
                return;
            }
        }
    }

    private void Sink(int index)
    {
        int largest, left = (index << 1) + 1;
        while (left < NowLength)
        {
            largest = left + 1 < NowLength && IsBetter(heap[left + 1], heap[left]) ? left + 1 : left;
            if (IsBetter(heap[index], heap[largest]))
            {
                largest = index;
            }

            if (largest == index) return;
            Swap(heap[largest], heap[index]);
            index = largest;
            left = (index << 1) + 1;
        }
    }

    private bool IsBetter(T p0, T p1)
    {
        return isReverse ? (p1.CompareTo(p0) < 0) : (p0.CompareTo(p1) < 0);
    }
}
