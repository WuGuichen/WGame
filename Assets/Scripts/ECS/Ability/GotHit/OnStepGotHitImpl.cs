using Weapon;

public class OnStepGotHitImpl : IGotHitService
{
    public bool OnGotHit(GameEntity entity, ContactInfo hitInfo)
    {
        return false;
    }

    public bool OnGotHit(GameEntity entity, SensorEntity sensor, int parts)
    {
        return false;
    }
}
