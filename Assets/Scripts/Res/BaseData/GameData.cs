using BaseData;
using SimpleJSON;
using WGame.Res;

public class GameData
{
    #if UNITY_EDITOR
    private static Tables _tables = new BaseData.Tables(file =>
    {
        if (YooassetManager.Inst.IsInitted)
        {
            return JSON.Parse(YooassetManager.Inst.LoadTextAssetSync(file));
        }
        return JSON.Parse(System.IO.File.ReadAllText(string.Format("Assets/Res/BaseDataJson/{0}.json", file),
            System.Text.Encoding.UTF8));
    });
    #else
    private static Tables _tables = new BaseData.Tables(file => 
                    JSON.Parse(YooassetManager.Inst.LoadTextAssetSync(file)));
    #endif
    public static Tables Tables => _tables;
}
