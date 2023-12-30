using System.Collections.Generic;
using UnityEngine;

public interface IVMService
{
    public void DoString(string str);
    public void Call(string name);
    public Symbol Resolve(string name);
    public void Set(string name, int value);
    public void Set(string name, bool value);
    public void Set(string name, float value, bool cached = true);
    public void Set(string name, string value);
    public void Set(string name, int[] value, bool cached = true);
    public void Set(string name, float[] value, bool cached = true);
    public void Set(string name, Symbol[] value, bool cached = true);
    public void Set(string name, Vector2 value, bool cached = true);
    public void Set(string name, Vector3 value, bool cached = true);
    public void Set(string name, Method value, bool cached = true);

    public void TriggerEvent(int id, List<Symbol> param);
    public WBTree AppendBehaviorTree(string name, GameObject obj);
    public WFSM GetFSM(string name);
    public void ReleaseWObject(WObject wfsm);
    
    GameEntity Entity { get; }

    public void CleanUp();
}
