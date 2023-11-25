using BaseData;
using SimpleJSON;
using WGame.Res;

public class GameData
{
    private static Tables _tables = new BaseData.Tables(file => 
                    JSON.Parse(YooassetManager.Inst.LoadTextAssetSync(file)));
    public static Tables Tables => _tables;
}
