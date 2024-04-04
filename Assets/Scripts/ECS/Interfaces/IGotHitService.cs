using Weapon;

public interface IGotHitService
{
    bool OnGotHit(GameEntity entity, ContactInfo hitInfo);
    bool OnGotHit(GameEntity entity, SensorEntity sensor, int parts);
}
