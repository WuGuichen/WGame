using System.Collections.Generic;
using WGame.Runtime;

public class FSMAgent
{
    private static readonly Stack<FSMAgent> _pool = new();

    public static FSMAgent Get(IVMService service)
    {
        if (_pool.Count > 0)
        {
            var item = _pool.Pop();
            item.InternalInit(service);
            return item;
        }

        return new FSMAgent(service);
    }

    public static void Push(FSMAgent agent)
    {
        agent.InternalDispose();
        _pool.Push(agent);
    }

    private IVMService _vmService;
    
    private FSMAgent(IVMService vmService)
    {
        InternalInit(vmService);
    }
    
    private readonly Dictionary<string, WFSM> _fsmDict = new();
    private readonly List<WFSM> _fsmList = new();

    private void InternalInit(IVMService vmService)
    {
        _vmService = vmService;
        EventCenter.AddListener(EventDefine.OnFSMHotUpdate, OnFSMHotUpdate);
    }

    private void OnFSMHotUpdate(WEventContext context)
    {
        RefreshFSM(context.pString);
    }

    private void RefreshFSM(string name)
    {
        if (_fsmDict.TryGetValue(name, out var fsm))
        {
            fsm.FSM.RequestExit();
            _fsmList.Remove(fsm);
            _vmService.ReleaseWObject(fsm);
            
            fsm = _vmService.GetFSM(name);
        }
        else
        {
            return;
        }

        InitFSM(ref fsm, name);
    }

    public void Trigger(string name, int type)
    {
        if (_fsmDict.TryGetValue(name, out var fsm))
        {
            fsm.FSM.Trigger(type);
        }
    }

    public void SetObject(string name)
    {
        if (_fsmDict.TryGetValue(name, out var fsm))
        {
            return;
        }
        fsm = _vmService.GetFSM(name);
        InitFSM(ref fsm, name);
    }
    
    private void InitFSM(ref WFSM fsm, string name)
    {
        if (fsm == null)
        {
            WLogger.Warning("生成FSM失败：" + name);
        }
        else
        {
            _fsmDict[name] = fsm;
            fsm.FSM.Init();
            _fsmList.Add(fsm);
        }
    }

    public void RegisterMethod()
    {
        
    }

    public void OnUpdate()
    {
        for (int i = 0; i < _fsmList.Count; i++)
        {
            _fsmList[i].FSM.OnLogic();
        }
    }

    private void InternalDispose()
    {
        EventCenter.RemoveListener(EventDefine.OnFSMHotUpdate, OnFSMHotUpdate);
        for (int i = 0; i < _fsmList.Count; i++)
        {
            _fsmList[i].FSM.RequestExit();
            _vmService.ReleaseWObject(_fsmList[i]);
        }
        _fsmList.Clear();
        _fsmDict.Clear();
        _vmService = null;
    }
}
