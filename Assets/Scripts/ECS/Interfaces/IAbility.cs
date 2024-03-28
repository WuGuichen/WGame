using WGame.Ability;

public interface IAbility
{
    bool Do(string name, bool unique =false);
    void Process(float deltaTime);
    void GenEntity(EntityMoveInfo info);
}
