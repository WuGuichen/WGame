using System.Collections.Generic;
using CleverCrow.Fluid.BTs.Trees;
using UnityEngine;

public class WBTree : WObject
{
    public int ObjectID { get; private set; }

    public List<int> CachedMethod { get; private set; }

    public List<int> CachedTable { get; private set; }

    public List<int> CachedFloat { get; private set; }

    private BehaviorTreeBuilder builder;
    public BehaviorTreeBuilder TREE_BUILDER => builder;
    private BehaviorTree _tree;
    public BehaviorTree TREE{
        get
        {
            if (_tree == null)
                _tree = builder.Build();
            return _tree;
        }
    }

    public WBTree(int objID, GameObject obj)
    {
        ObjectID = objID;
        builder = new BehaviorTreeBuilder(obj);
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
