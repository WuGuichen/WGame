using System.Collections.Generic;
using WGame.Runtime;

public class BTreeAgent
{
    private static readonly Stack<BTreeAgent> _pool = new();


    public static BTreeAgent Get(IVMService service)
    {
        if (_pool.Count > 0)
        {
            var item = _pool.Pop();
            item.InternalInit(service);
            return item;
        }

        return new BTreeAgent(service);
    }

    public static void Push(BTreeAgent agent)
    {
        agent.InternalDispose();
        _pool.Push(agent);
    }

    private IVMService _vmService;
    private MotionServiceImplementation motion;
    private UnityEngine.GameObject obj;
    
    private BTreeAgent(IVMService vmService)
    {
        InternalInit(vmService);
    }
    
    private readonly Dictionary<string, WBTree> _treeDict = new();
    
    private void InternalInit(IVMService vmService)
    {
        _vmService = vmService;
        obj = vmService.Entity.gameViewService.service.Model.gameObject;
        motion = vmService.Entity.linkMotion.Motion.motionService.service as MotionServiceImplementation;
        EventCenter.AddListener(EventDefine.OnBTreeHotUpdate, OnHotUpdate);
    }

    private void OnHotUpdate(TAny context)
    {
        RefreshBTree(context.AsString());
    }

    private void RefreshBTree(string name)
    {
        if (_treeDict.TryGetValue(name, out var tree))
        {
            tree.TREE.Reset();
            _vmService.ReleaseWObject(tree);

            tree = _vmService.AppendBehaviorTree(name, obj);
        }
        else
        {
            return;
        }
        
        InitBTree(ref tree, name);
    }

    private void InitBTree(ref WBTree tree, string name)
    {
        if (tree == null)
        {
            WLogger.Warning("生成BTree失败：" + name);
        }
        else
        {
            _treeDict[name] = tree;
        }
    }

    private WBTree SetObject(string name)
    {
        if (_treeDict.TryGetValue(name, out var tree))
        {
            return tree;
        }

        tree = _vmService.AppendBehaviorTree(name, obj);
        _treeDict[name] = tree;
        return tree;
    }

    public void TickTree(string name)
    {
        if (!_treeDict.TryGetValue(name, out var tree))
        {
            tree = SetObject(name);
        }

        if (tree != null)
        {
            tree.TREE.Tick();
            // motion.CurShowBehaviorTree = tree.TREE;
        }
    }

    private void InternalDispose()
    {
        obj = null;
        _vmService = null;
        motion = null;
        foreach (var kv in _treeDict)
        {
            kv.Value.TREE.Reset();
            _vmService.ReleaseWObject(kv.Value);
        }
        _treeDict.Clear();
    }
}
