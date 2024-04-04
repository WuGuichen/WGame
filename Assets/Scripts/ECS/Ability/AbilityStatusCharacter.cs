using System.Collections.Generic;
using WGame.Ability;

public class AbilityStatusCharacter : AbilityStatus
{
    #region pool

    public static AbilityStatusCharacter Empty()
    {
        var res = AbilityStatusCharacter.Get(null, null);
        Push(res);
        return res;
    }
    private static Stack<AbilityStatusCharacter> _pool = new();

    public static AbilityStatusCharacter Get(EventOwner owner, AbilityData abilityData)
    {
        if (_pool.Count > 0)
        {
            return _pool.Pop().Init(owner, abilityData);
        }

        return new AbilityStatusCharacter(owner, abilityData);
    }

    public static void Push(AbilityStatusCharacter status)
    {
        status.Reset();
        status._owner = null;
        _pool.Push(status);
    }
    
    private AbilityStatusCharacter(EventOwner owner, AbilityData abilityData)
    {
        Init(owner, abilityData);
    }

    private AbilityStatusCharacter Init(EventOwner owner, AbilityData abilityData)
    {
        _owner = owner;
        _cameraService = Contexts.sharedInstance.meta.mainCameraService.service;
        Initialize(abilityData);
        return this;
    }
    
    #endregion

    private EventOwner _owner;
    private IMotionService _motionService;
    private ICameraService _cameraService;
    private int _rotCameraCount = 0;
    private int _moveCameraCount = 0;
    
    protected override void OnStart()
    {
    }
    
    public bool TryGetProperty(string name, out TAny value)
    {
        return _ability.Context.TryGetProperty(name, out value);
    }

    protected override void OnExitDuration(IEventData eventData, bool isBreak)
    {
        eventData.Exit(_owner, isBreak);
    }

    protected override void OnProcessDuration(IEventData eventData, float deltaTime, int duration, int totalTime)
    {
        eventData.Duration(_owner, deltaTime ,duration, totalTime);
    }

    protected override void OnEnterDuration(IEventData eventData)
    {
        eventData.Enter(_owner);
    }

    protected override void OnTriggerSignal(IEventData eventData)
    {
        eventData.Enter(_owner);
    }

    protected override void OnEnd()
    {
        base.OnEnd();
    }
}
