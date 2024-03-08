using WGame.Ability;
using WGame.Res;

public class AbilityAssetLoader : IGameAssetLoader
{
    public string LoadTextAsset(string filePath)
    {
        return YooassetManager.Inst.LoadTextAssetSync(filePath);
    }

    public string[] GetAbilityGroups()
    {
        return new string[] { "TestGroup" };
    }
}
