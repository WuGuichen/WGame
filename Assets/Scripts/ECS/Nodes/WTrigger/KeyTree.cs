using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WGame.Trigger;

public class KeyTree
{
    #region pool

    private static Stack<KeyTree> _pool = new Stack<KeyTree>();

    public static KeyTree Get()
    {
        if (_pool.Count > 0)
        {
            var node = _pool.Pop();
            node.children.Clear();
            node.triggerList = null;
            return node;
        }

        return new KeyTree();
    }

    public static void Push(KeyTree node) => _pool.Push(node);

    public KeyTree Build(WtEventType type)
    {
        var root = Get();
        return root;
    }

    #endregion

    private SparseSet<KeyTree> children = new SparseSet<KeyTree>();
    private List<WTrigger> triggerList;
    private Dictionary<int, WTrigger> triggerDict;

    public void AddNode(int key, out KeyTree child)
    {
        if (children.TryGet(key, out child))
        {
            return;
        }
        child = Get();
        children.Add(key, child);
    }

    public bool RemoveNode(int key)
    {
        children.Remove(key);
        return children.IsEmpty;
    }

    public bool IsContain(int key) => children.IsContain(key);

    public bool TryGet(int key, out KeyTree node)
    {
        if (children.TryGet(key, out node))
            return true;
        return false;
    }
    
    public Dictionary<int,WTrigger> GetValue(int key1, int key2)
    {
        return triggerDict;
        // if (triggers == null)
        //     return null;
        // if (triggers.TryGetValue(key1, out var dict))
        // {
        //     if (dict.TryGetValue(key2, out var trigger))
        //     {
        //         return trigger;
        //     }
        // }
        //
        // return null;
    }

    public void AddTrigger(int key1, int key2, WTrigger trigger)
    {
        if (triggerDict == null)
            triggerDict = new Dictionary<int, WTrigger>();
        triggerDict[trigger.triggerIndex] = trigger;
        // if (triggers == null)
        // {
        //     triggers = new Dictionary<int, Dictionary<int, List<WTrigger>>>();
        // }
        //
        // if (!triggers.TryGetValue(key1, out var subTypeDict))
        // {
        //     subTypeDict = new Dictionary<int, List<WTrigger>>()
        //     {
        //         {key2, new List<WTrigger>()}
        //     };
        //     subTypeDict[key2].Add(trigger);
        //     triggers.Add(key1, subTypeDict);
        // }
        // else
        // {
        //     subTypeDict[key2].Add(trigger);
        // }
    }

    public bool RemoveTrigger(int key1, int key2, WTrigger trigger)
    {
        if (triggerDict == null)
            return true;
        triggerDict.Remove(trigger.triggerIndex);
        if (triggerDict.Count == 0)
            return true;
        return false;
        // if (!triggers.TryGetValue(key1, out var subTypeDict))
        // {
        //     return true;
        // }
        //
        // subTypeDict[key2].Remove(trigger);
        //
        // if(subTypeDict[key2].Count == 0)
        //     subTypeDict.Remove(key2);
        // if (subTypeDict.Count == 0)
        // {
        //     triggers.Remove(key1);
        //     if (triggers.Count == 0)
        //     {
        //         triggers = null;
        //         return true;
        //     }
        // }
        //
        // return false;
    }
}
