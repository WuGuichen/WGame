using UnityTimer;
using WGame.Notice;

public class RecieverBeHittedOnStep : IReciever
{
    public RecieverBeHittedOnStep(int key) : base(key, MessageDB.BeHittedID, 9999)
    {
    }

    public override void OnAdded(GameEntity entity)
    {
        entity.linkAbility.Ability.abilityEvade.service.Enable();
        WLogger.Print("Added");
    }

    public override void OnRemoved(GameEntity entity)
    {
        entity.linkAbility.Ability.abilityEvade.service.Disable();
        WLogger.Print("Remove");
    }

    public override void OnTrigger(GameEntity entity, IMessage message)
    {
        UnityEngine.Time.timeScale = 0.2f;
        WLogger.Print("极限");
        Timer.Register(0.1f, () =>
        {
            UnityEngine.Time.timeScale = 1f;
        });
    }

    public override bool CheckCondition(IMessage message)
    {
        var msg =(MessageDB.Define.BeHitted)message;
        if (msg.hitInfo.part == EntityPartType.Evasion)
        {
            return true;
        }

        return false;
    }
}
