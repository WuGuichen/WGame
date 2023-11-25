using System.Collections.Generic;
using CleverCrow.Fluid.BTs.Trees;
using UnityHFSM;
using WGame.Runtime;

public class WBTreeMgr : Singleton<WBTreeMgr>
{
    private Dictionary<string, BehaviorTreeBuilder> _behaviorTreeBuilders = new Dictionary<string, BehaviorTreeBuilder>();

    private StateMachine<int, int, int> _fsm;

    private Dictionary<string, List<string>> _fsmTree = new Dictionary<string, List<string>>();

    public void SetFsmTree(string child, string parent)
    {
        if (_fsmTree.TryGetValue(child, out var list))
        {
            if(list.Contains(parent)== false)
                list.Add(parent);
        }
        else
        {
            list = new List<string>() { parent };
            _fsmTree[child] = list;
        }
    }

    public void RefreshFSM(string text)
    { 
        EventCenter.Trigger(EventDefine.OnFSMHotUpdate, WEventContext.Get(text));
        
        if (_fsmTree.TryGetValue(text, out var list))
        {
            for (int i = 0; i < list.Count; i++)
            {
                EventCenter.Trigger(EventDefine.OnFSMHotUpdate, WEventContext.Get(list[i]));
            }
        }
    }
    
    public void AddTree(string name, BehaviorTreeBuilder builder)
    {
        _behaviorTreeBuilders[name] = builder;
    }

    public void AppendTree(string name, ref BehaviorTreeBuilder builder)
    {
        if (_behaviorTreeBuilders.TryGetValue(name, out var tree))
        {
            builder.Splice(tree.Build());
        }
    }
}
