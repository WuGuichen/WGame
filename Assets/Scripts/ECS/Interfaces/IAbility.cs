using WGame.Ability;

public interface IAbility
{
    void Do(string name);
    void Process(float deltaTime);
    void GenEntity(EntityMoveInfo info);
}
