public class InstaceDB<T> where T : new()
{
    private T[] list = new T[8];
    private int count = 0;
    private System.Collections.Generic.Stack<int> emptyStk = new();
    private readonly T empty = default;

    public int Register(T entity)
    {
        int instanceID = count;
        if (emptyStk.Count > 0)
        {
            instanceID = emptyStk.Pop();
        }
        else
        {
            count++;
        }
        Ensure();
        list[instanceID] = entity;
        return instanceID;
    }

    private void Ensure()
    {
        int len = list.Length;
        if (count >= len)
        {
            var tmp = new T[len*2];
            for (int i = 0; i < len; i++)
            {
                tmp[i] = list[i];
            }

            list = tmp;
        }
    }

    public void Cancel(int instID)
    {
        emptyStk.Push(instID);
        list[instID] = empty;
    }

    public void Clear()
    {
        emptyStk.Clear();
        list = new T[1];
        count = 0;
    }

    public T this[int instID] => list[instID];
}
