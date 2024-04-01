using WGame.Ability;

public interface IAbility
{
    public EventOwner Owner { get; }
    BuffManager BuffManager { get; }
    bool IsProcessAbility { get; }
    bool SwitchMotionAbility(int motionType);
    void Process(float deltaTime);
    void GenEntity(EntityMoveInfo info);
}
