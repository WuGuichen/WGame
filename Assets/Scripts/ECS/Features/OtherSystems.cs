public class OtherSystems : Feature
{
	public OtherSystems(Contexts contexts)
	{
		// 注意系统顺序
		Add(new UpdateCharacterDataSystem(contexts));
		Add(new AnimSpeedSystem(contexts));
		
		Add(new CharacterOnGroundSystem(contexts));
		Add(new UpdateDeviceInputSignalSystem(contexts));
		Add(new UpdateFocusInputSystem(contexts));
		Add(new FocusEntitySystem(contexts));
		Add(new AIAgentUpdateSystem(contexts));
		Add(new RefreshCharacterUISystem(contexts));
		Add(new ProcessAbilitySystem(contexts));
	}
}
