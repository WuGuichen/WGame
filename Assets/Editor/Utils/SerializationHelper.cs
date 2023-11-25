using System.IO;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SerializationHelper
{
    public static DataFormat dataType = DataFormat.JSON;

    public static string MotionDataPath = "Assets/Res/MotionData/";

    public static void SerializeData<T>(string dirPath, ref T data, string fileName)
    {
        string suffix = dataType switch
        {
            DataFormat.Binary => ".bytes",
            DataFormat.JSON => ".txt",
            _ => ""
        };
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        var bytes = Sirenix.Serialization.SerializationUtility.SerializeValue(data, dataType);
        var filePath = dirPath + fileName + suffix;
        File.WriteAllBytes(filePath, bytes);
        WLogger.Info("序列化成功："+ filePath);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public static byte[] LoadSerializeData(string filePath)
    {
#if UNITY_EDITOR
        var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
        return textAsset.bytes;
#else
        return null;
#endif
    }

    public static void DesrializeData<T>(string filePath, out T data)
    {
        var bytes = LoadSerializeData(filePath);
        data = Sirenix.Serialization.SerializationUtility.DeserializeValue<T>(bytes, dataType);
    }

    public static void DeserializeValue<T>(byte[] bytes, out T data)
    {
        data = Sirenix.Serialization.SerializationUtility.DeserializeValue<T>(bytes, dataType);
    }
}
