using FairyGUI.Dynamic;
using UnityEngine;
using WGame.Runtime;

public class YooassetUIAssetLoader : IUIAssetLoader
{
    public void LoadUIPackageBytesAsync(string packageName, out byte[] bytes, out string assetNamePrefix)
    {
        var assetPath = packageName + "_fui";
        bytes = YooassetManager.Inst.LoadBytesSync(assetPath);
        assetNamePrefix = packageName;
    }
    
    // public void LoadTextureAsync(string packageName, string assetName, string extension, )
    public void LoadUIPackageBytesAsync(string packageName, LoadUIPackageBytesCallback callback)
    {
        var assetPath = packageName;
        
        YooassetManager.Inst.LoadBytesAsync(assetPath + "_fui", bytes =>
        {
            callback(bytes, assetPath);
        });
    }

    public void LoadUIPackageBytes(string packageName, out byte[] bytes, out string assetNamePrefix)
    {
        var assetPath = packageName;
        bytes = YooassetManager.Inst.LoadBytesSync(assetPath + "_fui");
        assetNamePrefix = assetPath;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void LoadTextureAsync(string packageName, string assetName, string extension, LoadTextureCallback callback)
    {
        YooassetManager.Inst.LoadTextureAsync(assetName, texture =>
        {
            callback.Invoke(texture);
        });
    }

    public void UnloadTexture(Texture texture)
    {
        YooassetManager.Inst.UnloadAsset(texture);
    }

    public void LoadAudioClipAsync(string packageName, string assetName, string extension, LoadAudioClipCallback callback)
    {
        YooassetManager.Inst.LoadAudioClipAssetAsync(assetName, clip =>
        {
            callback.Invoke(clip);
        });
    }

    public void UnloadAudioClip(AudioClip audioClip)
    {
        YooassetManager.Inst.UnloadAsset(audioClip);
    }
}
