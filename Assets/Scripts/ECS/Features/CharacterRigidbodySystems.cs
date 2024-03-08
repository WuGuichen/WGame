public class CharacterRigidbodySystems : Feature
{
    public CharacterRigidbodySystems(Contexts contexts)
    {
		Add(new MoveCharacterSystem(contexts));
		Add(new RotateCharacterSystem(contexts));
		Add(new UpdateMoveDirectionSystem(contexts));
    }
}
