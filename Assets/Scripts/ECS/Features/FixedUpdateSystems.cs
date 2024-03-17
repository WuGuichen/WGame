public class FixedUpdateSystems : Feature
{
    public FixedUpdateSystems(Contexts contexts)
    {
		Add(new MoveCharacterSystem(contexts));
		Add(new RotateCharacterSystem(contexts));
		Add(new UpdateMoveDirectionSystem(contexts));
		
        Add(new MoveTriggerObjectSystem(contexts));
    }
}
