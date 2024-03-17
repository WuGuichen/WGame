using Weapon;

public interface IGotHitService
{
    void OnGotHit(GameEntity entity, ContactInfo hitInfo);
    void OnGotHit(GameEntity entity, SensorEntity sensor, int parts);
    void OnHitTarget(GameEntity entity, ContactInfo hitInfo);
}
