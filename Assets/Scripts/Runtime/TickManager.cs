using WGame.Runtime;

public delegate void TickCallback(float deltaTime);
public delegate void TickCallback1();
public delegate void TickCallbackSecond();
public class TickManager : Singleton<TickManager>
{
    private TickCallback tickCallback;
    private TickCallback1 tickLateUpdate;
    private TickCallback1 tickCallback1;
    private TickCallbackSecond tickCallbackSecond;
    private float timer = 0;

    public void AddTick(TickCallback callback)
    {
        tickCallback -= callback;
        tickCallback += callback;
    }
    
    public void AddTick(TickCallback1 callback)
    {
        tickCallback1 -= callback;
        tickCallback1 += callback;
    }

    public void AddTickLateUpdate(TickCallback1 callback)
    {
        tickLateUpdate -= callback;
        tickLateUpdate += callback;
    }

    public void RemoveTickLateUpdate(TickCallback1 callback)
    {
        tickLateUpdate -= callback;
    }
    
    public void RemoveTick(TickCallback1 callback)
    {
        tickCallback1 -= callback;
    }
    
    public void RemoveTick(TickCallback callback)
    {
        tickCallback -= callback;
    }
    
    public void AddTickSecond(TickCallbackSecond callback)
    {
        tickCallbackSecond -= callback;
        tickCallbackSecond += callback;
    }
    
    public void RemoveTickSecond(TickCallbackSecond callback)
    {
        tickCallbackSecond -= callback;
    }

    public void UpdateTick(float deltaTime)
    {
        tickCallback?.Invoke(deltaTime);
        tickCallback1?.Invoke();
        if (timer <= 0)
        {
            tickCallbackSecond?.Invoke();
            timer = 1f;
        }
        else
        {
            timer -= deltaTime;
        }
    }

    public void UpdateLate()
    {
        tickLateUpdate?.Invoke();
    }

    public void Dispose()
    {
        tickCallback = null;
        tickCallback1 = null;
        tickCallbackSecond = null;
    }
}
