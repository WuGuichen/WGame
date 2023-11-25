using System.Collections.Generic;
using UnityHFSM;

public class WFSM : WObject
{
    public int ObjectID { get; private set; }

    public List<int> CachedMethod { get; private set; }

    public List<int> CachedTable { get; private set; }

    public List<int> CachedFloat { get; private set; }

    private StateMachine<int, int, int> fsm;
    public StateMachine<int, int, int> FSM => fsm;

    public WFSM(int objId)
    {
        fsm = new StateMachine<int, int, int>();
        ObjectID = objId;
    }

    public void SetCacheFloat(List<int> list)
    {
        CachedFloat = list;
    }
    
    public void SetCacheMethod(List<int> list)
    {
        CachedMethod = list;
    }
    
    public void SetCacheTable(List<int> list)
    {
        CachedTable = list;
    }

    public void Dispose()
    {
        
    }
}
