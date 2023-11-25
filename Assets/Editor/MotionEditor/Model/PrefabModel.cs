using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Motion;
using UnityEditor;
using UnityEngine;

public class PrefabModel
{
    private static readonly string relativeMotionPath = "Assets/Res/Motions/";
    public static readonly string motionPath = Application.dataPath + "/Res/Motions";
    
    public enum TableType
    {
        Player,
        Enemy,
    }

    public TableType currentType;

    static EventNode copiedControl;

    internal static void CopyControl(EventNode control)
    {
        copiedControl = control;
    }

    internal static void PasteControl(WindowState state)
    {
        state.AddNodeControl(copiedControl);
    }

    public static int currentMotionID;
    
    
    private static EventNodeScriptableObject _currentConfig;
    public static EventNodeScriptableObject currentConfig
    {
        get
        {
            return _currentConfig;
        }
        set
        {
            _currentConfig = value;
            RefreshFileName();
        }
    }
    public static string currentConfigName;

    public static GameObject GetPrefab(TableType type, int id)
    {
        var fileName = "todo";
        var path = "Assets/Prefabs/Character/{0}.prefab";
        var go = (GameObject) AssetDatabase.LoadAssetAtPath(string.Format(path,fileName),typeof(GameObject));
        return go;
    }

    public static string GetMotionName(int id)
    {
        return "todo";
    }

    public static string[] GetMotionList()
    {
        if (Directory.Exists(motionPath))
        {
            DirectoryInfo info = new DirectoryInfo(motionPath);
            FileInfo[] files = info.GetFiles("*.asset");
            return files.Select(r => Path.GetFileNameWithoutExtension(r.Name)).ToArray();
        }

        return null;
    }

    public static int GenerateMotionID()
    {
        if (Directory.Exists(motionPath))
        {
            var Ids = GetMotionIDs();
            if (Ids.Count > 0)
                return Ids[^1] + 1;
            else
                return 20000;
        }

        return 0;
    }

    public static List<int> GetMotionIDs()
    {
        var Ids = new List<int>();
        if (Directory.Exists(motionPath))
        {
            DirectoryInfo info = new DirectoryInfo(motionPath);
            FileInfo[] files = info.GetFiles("*.asset");
            for (var i = 0; i < files.Length; i++)
            {
                var path = relativeMotionPath + files[i].Name;
                var cfg = AssetDatabase.LoadAssetAtPath<EventNodeScriptableObject>(path);
                Ids.Add(cfg.UID);
            }

            Ids.Sort();
        }

        return Ids;
    }

    static void RefreshFileName()
    {
        if (!currentConfig)
            return;
        if (Directory.Exists(motionPath))
        {
            DirectoryInfo info = new DirectoryInfo(motionPath);
            FileInfo[] files = info.GetFiles("*.asset");
            for (var i = 0; i < files.Length; i++)
            {
                var path = relativeMotionPath + files[i].Name;
                var cfg = AssetDatabase.LoadAssetAtPath<EventNodeScriptableObject>(path);
                if (cfg.UID == currentConfig.UID)
                {
                    currentConfigName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(files[i].Name));
                }
            }
        }
    }
    public static EventNodeScriptableObject GetMotionByID(int id)
    {
        if (Directory.Exists(motionPath))
        {
            DirectoryInfo info = new DirectoryInfo(motionPath);
            FileInfo[] files = info.GetFiles("*.asset");
            for (var i = 0; i < files.Length; i++)
            {
                var path = relativeMotionPath + files[i].Name;
                var cfg = AssetDatabase.LoadAssetAtPath<EventNodeScriptableObject>(path);
                if (cfg.UID == id)
                {
                    return cfg;
                }
            }
        }

        return null;
    }

    public static EventNodeScriptableObject GetMotionAssetByName(string name)
    {
        var cfg = AssetDatabase.LoadAssetAtPath<EventNodeScriptableObject>(relativeMotionPath+name+".asset");
        return cfg;
    }

    public static string GetCurrentMotionPath()
    {
        return Path.Combine(relativeMotionPath, currentConfigName) + ".asset";
    }
}
