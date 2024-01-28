using System;
using Motion;
using UnityEngine;

public interface IFactoryService
{
    public void InitSceneObjectRoot(Transform sceneRoot);
    public AnimationClip GetAnimationClip(int clipID);
    public void LoadAnimationClip(string clipName, Action<AnimationClip> callback);
    public EventNodeScriptableObject GetMotion(int motionID);
    public void Dispose();
    public void GenCharacter(int charID, Vector3 pos, Quaternion rot, out GameEntity entity, Action<GameEntity> callback =null);
    public void GenCharacter(GameObject obj);

    public void GenWeaponEntity(int weaponID, out WeaponEntity weapon);
    public void SetWeaponDrop(int entityID, Vector3 pos, Quaternion rot, Vector3 scale);
    public void SetWeaponDrop(WeaponEntity weapon, Vector3 pos, Quaternion rot, Vector3 scale);
    public void SetWeaponEquipTo(WeaponEntity weapon, GameEntity entity);
    public GameEntity SelectRandomGenCharacter();
    public GameEntity GetGameEntity(int instId);
}
