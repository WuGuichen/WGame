using System.Collections.Generic;

public interface WObject
{
    int ObjectID { get; }
    List<int> CachedMethod { get; }
    List<int> CachedTable { get; }
    List<int> CachedFloat { get; }
    void Dispose();
}
