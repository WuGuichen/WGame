public interface IHeapItem<T> : System.IComparable<T>
{
    public int HeapIndex { get; set; }
}
