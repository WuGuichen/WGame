using System;
using UnityEngine;
using WGame.Ability;
using WGame.Runtime;

public class AbilityAssetLoader : IGameAssetLoader
{
    public void LoadAnimationClip(string filePath, Action<AnimationClip> callback)
    {
        YooassetManager.Inst.LoadAnimationClipAsync(filePath, callback);
    }

    public string LoadTextAsset(string filePath)
    {
        return YooassetManager.Inst.LoadTextAssetSync(filePath);
    }

    public string[] GetAbilityGroups()
    {
        return new string[] { "TestGroup" };
    }
}
