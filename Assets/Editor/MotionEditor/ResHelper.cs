using System.Linq;
using BaseData;
using Motion;
using SimpleJSON;
using UnityEngine;
using UnityEditor;

public class ResHelper
{
    private static string jsonPath = Application.dataPath + "/Res/BaseDataJson";
    public static Tables gameData = new BaseData.Tables(file => 
                    JSONNode.Parse(System.IO.File.ReadAllText($"{jsonPath}/{file}.json", System.Text.Encoding.UTF8)));
    public static int[] clipList = gameData.TbClip.DataList.Select(r => r.Id).ToArray();
    public static string[] clipNameList = gameData.TbClip.DataList.Select(r => r.Path).ToArray();
    private const string AnimClipBasePath = "Assets/Res/Animations/{0}.anim";
    
    public static AnimationClip LoadAnimClip(int id)
    {
        var path = gameData.TbClip.Get(id).Path;
        path = string.Format(AnimClipBasePath, path);
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        return clip;
    }

    public static EventNodeScriptableObject LoadMotion(int id)
    {
        return PrefabModel.GetMotionByID(id);
    }

    public static EventNodeScriptableObject LoadMotion(string name)
    {
        return PrefabModel.GetMotionAssetByName(name);
    }
    
    public static void Init()
    {gameData = new BaseData.Tables(file => 
                    JSONNode.Parse(System.IO.File.ReadAllText($"{jsonPath}/{file}.json", System.Text.Encoding.UTF8)));
    
        clipList = gameData.TbClip.DataList.Select(r => r.Id).ToArray();
        clipNameList = gameData.TbClip.DataList.Select(r => r.Path).ToArray();
    }
}
