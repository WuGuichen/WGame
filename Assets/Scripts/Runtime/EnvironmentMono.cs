using System;
using UnityEngine;
using WGame.Runtime;

public class EnvironmentMono : MonoBehaviour
{
    [SerializeField]
    private Transform obstacle;
    public Transform Obstacle => obstacle;
    [SerializeField]
    private Transform items;
    public Transform Items => items;
    [SerializeField]
    private Transform characters;
    public Transform Characters => characters;
    [SerializeField]
    private Transform characterRoot;
    public Transform CharacterRoot => characterRoot;
    [SerializeField]
    private Transform triggerObjRoot;
    public Transform TriggerObjRoot => triggerObjRoot;
    [SerializeField]
    private Transform cameraRoot;
    public Transform CameraRoot => cameraRoot;

    private void Awake()
    {
        if (YooassetManager.Inst == null)
        {
            gameObject.AddComponent<YooassetManager>();
            EventCenter.AddListener(EventDefine.OnGameAssetsManagerInitted, OnAssetInitted);
        }
    }

    private void OnAssetInitted()
    {
        EventCenter.AddListener(EventDefine.OnGameSystemsInitted, OnGameSystemsInitialized);
        GameSceneMgr.Inst.SetEnvironment();
        EventCenter.RemoveListener(EventDefine.OnGameAssetsManagerInitted, OnAssetInitted);
    }

    private void OnGameSystemsInitialized()
    {
        EventCenter.Trigger(EventDefine.OnEnterGameMainView);
        EventCenter.RemoveListener(EventDefine.OnGameSystemsInitted, OnGameSystemsInitialized);
    }
}
