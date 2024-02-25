using UnityTimer;
using WGame.Notice;

public class RecieverBeHittedOnStep : IReciever
{
    public RecieverBeHittedOnStep(int key, int times = 9999) : base(key, MessageDB.BeHittedID, times)
    {
    }

    public override void OnAdded()
    {
        base.OnAdded();
    }

    public override void OnRemoved()
    {
        base.OnRemoved();
    }

    public override void OnTrigger()
    {
        UnityEngine.Time.timeScale = 0.1f;
        WLogger.Print("极限");
        Timer.Register(0.2f, () =>
        {
            UnityEngine.Time.timeScale = 1f;
        });
    }

    public override bool CheckCondition(IMessage message)
    {
        return true;
    }
}
