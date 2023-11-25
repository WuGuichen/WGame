using System.Collections.Generic;

public class WObjectPool
{
    private int objectCount = 0;
    private Stack<int> emptyIDs = new Stack<int>();

    public WObjectPool()
    {
        // var fsm = new WFSM();
    }

    private int GenObjectID()
    {
        if (emptyIDs.Count > 0)
        {
            return emptyIDs.Pop();
        }

        return ++objectCount;
    }

    public WFSM GetWFSM()
    {
        return new WFSM(GenObjectID());
    }

    public WBTree GetWBTree(UnityEngine.GameObject obj)
    {
        return new WBTree(GenObjectID(), obj);
    }

    public void PushObj(WObject wobj, CachedDefinition definition)
    {
        for (int i = 0; i < wobj.CachedMethod.Count; i++)
        {
            definition.ReleaseMethod(wobj.CachedMethod[i]);
        }
        for (int i = 0; i < wobj.CachedFloat.Count; i++)
        {
            definition.ReleaseFloat(wobj.CachedFloat[i]);
        }
        for (int i = 0; i < wobj.CachedTable.Count; i++)
        {
            definition.ReleaseTable(wobj.CachedTable[i]);
        }
        wobj.Dispose();
    }
}
