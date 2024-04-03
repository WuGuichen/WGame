using System.Collections.Generic;
using BaseData.Character;
using NativeQuadTree;
using TWY.Physics;
using UnityEngine;
using WGame.Runtime;

public class EntityUtils
{
    private static readonly int playerLayer = LayerMask.NameToLayer("Player");
    private static readonly int enemyLayer = LayerMask.NameToLayer("Enemy");
    private static readonly int enemySensorLayer = LayerMask.NameToLayer("EnemyHitSensor");
    private static readonly int playerSensorLayer = LayerMask.NameToLayer("PlayerHitSensor");
    
    // 约定必须小于自定义ID且大于最大存在实体数
    public const int CharacterBaseID = 9000000;   
    public const int WeaponBaseID = 9000000;

    private static IFactoryService _factory;

    private static WBVH<IGameViewService> _BVHEnemy =
        new WBVH<IGameViewService>(new WBVHEntityAdapter(Camp.Red), new List<IGameViewService>());

    private static Dictionary<int, SensorMono> _colliderToEntityDict = new();
    private static Dictionary<int, AbilityTriggerInfo> _triggerToEntityDict = new();
    
    public static void RegisterTrigger(int triggerInstId, AbilityTriggerInfo info)
    {
        _triggerToEntityDict[triggerInstId] = info;
    }
    
    public static void CancelTrigger(int triggerInstId)
    {
        _triggerToEntityDict.Remove(triggerInstId);
    }
    
    public static bool TryGetAbilityTrigger(int triggerInstId, out AbilityTriggerInfo info)
    {
        return _triggerToEntityDict.TryGetValue(triggerInstId, out info);
    }

    public static void RegisterCollider(SensorMono mono, GameEntity entity)
    {
        _colliderToEntityDict[mono.ColliderId] = mono;
    }

    public static void CancelCollider(SensorMono mono)
    {
        _colliderToEntityDict.Remove(mono.ColliderId);
    }

    public static bool TryGetEntitySensorMono(int colliderId, out SensorMono sensorMono)
    {
        return _colliderToEntityDict.TryGetValue(colliderId, out sensorMono);
    }

    public static void RandomKillCharacter()
    {
        var factory = Contexts.sharedInstance.meta.factoryService.instance;
        var entity = factory.SelectRandomGenCharacter();
        if (entity != null)
        {
            entity.isDestroyed = true;
        }
        else
        {
            entity = GetCameraEntity();
            if (entity != null)
            {
                entity.isDestroyed = true;
            }
        }
    }

    public static WBVH<IGameViewService> BvhRed
    {
        get
        {
            if(_BVHEnemy == null)
                _BVHEnemy = new WBVH<IGameViewService>(new WBVHEntityAdapter(Camp.Red), new List<IGameViewService>());
            return _BVHEnemy;
        }
    }

    private static IFactoryService FactoryService
    {
        get
        {
            if (_factory == null)
                _factory = Contexts.sharedInstance.meta.factoryService.instance;
            return _factory;
        }
    }

    public static void SetEntityCamp(ref GameEntity entity)
    {
        var camp = entity.characterInfo.value.camp;
        switch (camp)
        {
            case Camp.Red:
                entity.isCampRed = true;
                return;
            case Camp.White:
                entity.isCampWhite = true;
                return;
            default:
                break;
        }
    }

    public static Vector3 GetCameraPos()
    {
        var cam = Contexts.sharedInstance.meta.mainCameraService.service.Camera;
        return cam.position;
    }

    public static GameEntity GetCameraEntity()
    {
        return GetGameEntity(ActionHelper.CurCameraEntityID);
    }

    // public static void DropEntityWeapon(GameEntity entity)
    // {
        // if (entity.hasLinkWeapon)
        // {
        //     FactoryService.SetWeaponDrop(entity.linkWeapon.Weapon, entity.position.value + entity.gameViewService.service.Model.forward, Quaternion.identity, Vector3.one);
        // }
    // }

    public static Vector3 GetCameraCharacterPos()
    {
        var entity = Contexts.sharedInstance.game.GetEntityWithEntityID(ActionHelper.CurCameraEntityID);
        if (entity != null && entity.isEnabled)
        {
            return entity.position.value;
        }

        return GetCameraPos();
    }
    
    public static Vector3 GetCameraFwdDir()
    {
        var cam = Contexts.sharedInstance.meta.mainCameraService.service.Camera;
        var dir = cam.forward;
        dir.y = 0;
        return dir.normalized;
    }
    
    public static Vector3 GetCameraRightDir()
    {
        var cam = Contexts.sharedInstance.meta.mainCameraService.service.Camera;
        var dir = cam.right;
        dir.y = 0;
        return dir.normalized;
    }

    public static Vector3 GetRandomPositionAroundCharacter(float radius = 5f, float offsetFwd = 0f, float offsetRight = 0f)
    {
        var charPos = GetCameraCharacterPos();
        var pos = Random.insideUnitSphere * radius + charPos;
        pos.y = charPos.y;
        if(Mathf.Abs(offsetFwd) > 0.1f)
            pos += GetCameraFwdDir()*offsetFwd;
        if(Mathf.Abs(offsetRight) > 0.1f)
            pos += GetCameraRightDir() * offsetRight;
        return pos;
    }

    public static void SelectRandomCharacter()
    {
        var factory = Contexts.sharedInstance.meta.factoryService.instance;
        var entity = factory.SelectRandomGenCharacter();
        if (entity != null)
        {
            ActionHelper.DoSetCharacterCameraByID(entity.entityID.id);
        }
    }

    private static void InitCharGroup(string name)
    {
	    var root = GameSceneMgr.Inst.editCharacterRoot;
        
        YooassetManager.Inst.LoadGameObject(name, o =>
        {
            var characterRoot = o.transform;
            characterRoot.parent = root;
            characterRoot.position = Vector3.zero;
            
            for (int i = 0; i < characterRoot.childCount; i++)
            {
                var child = characterRoot.GetChild(i).gameObject;
                var entityId = int.Parse(child.name);
                var entity = GetGameEntity(entityId);
                if (child.activeSelf && entity == null)
                {
                    Contexts.sharedInstance.meta.factoryService.instance.GenCharacter(child.gameObject);
                }

                if (entityId == 10000001)
                {
                    ActionHelper.DoSetCharacterCameraByID(10000001);
                }
            }
        });
    }
    public static void InitCharacterRoot()
    {
        // InitCharGroup("WhiteGroup");
        // InitCharGroup("RedGroup");
    }

    public static void GenCharacter(int type = 2, bool setCamera = false)
    {
        int num = Random.Range(1, 3);
        num = type;
        int charId = num;
        FactoryService.GenCharacter(charId, GetRandomPositionAroundCharacter(), Quaternion.identity, out var entity,
            gameEntity =>
            {
                if (setCamera)
                {
                    ActionHelper.DoSetCharacterCameraByID(gameEntity.instanceID.ID);
                }
            });
    }

    public static void GenRandomWeapon()
    {
        var factory = FactoryService;
        int weaponId = 1;
        // factory.GenWeaponEntity(weaponId, out var weapon);
        // factory.SetWeaponDrop(weapon, GetRandomPositionAroundCharacter(), Quaternion.identity, Vector3.one);
        var entity = GetCameraEntity();
        ActionHelper.DoDropObject(new DropObjectInfo(1), entity.gameViewService.service.FocusPoint, GetRandomPositionAroundCharacter());
    }

    public static int GetTargetSensorLayer(GameEntity entity)
    {
        if (entity.characterInfo.value.camp == Camp.Red)
        {
            return playerSensorLayer;
        }
        else if (entity.characterInfo.value.camp == Camp.White)
        {
            return enemySensorLayer;
        }
        return 0;
    }

    public static int GetLayer(GameEntity entity)
    {
        if (entity.characterInfo.value.camp == Camp.Red)
        {
            return enemyLayer;
        }
        else if (entity.characterInfo.value.camp == Camp.White)
        {
            return playerLayer;
        }
        return 0;
    }
    
    public static int GetSensorLayer(GameEntity entity)
    {
        if (entity.characterInfo.value.camp == Camp.Red)
        {
            return enemySensorLayer;
        }
        else if (entity.characterInfo.value.camp == Camp.White)
        {
            return playerSensorLayer;
        }
        return 0;
    }

    public static void Dispose()
    {
        _factory = null;
        _BVHEnemy = null;
    }

    public static GameEntity GetGameEntity(int id)
    {
        if (id < CharacterBaseID)
        {
            return FactoryService.GetGameEntity(id);
        }
        else
        {
            return Contexts.sharedInstance.game.GetEntityWithEntityID(id);
        }
    }

    public static GameEntity[] GetGameEntities()
    {
        return Contexts.sharedInstance.game.GetEntities();
    }

    // /// <summary>
    // /// 获取两个实体的距离(优先获取上次获取的数据)
    // /// </summary>
    // public static float GetDist(GameEntity red, GameEntity white)
    // {
    //     var redService = red.linkSensor.Sensor.detectorCharacterService.service;
    //     if (redService.TryGetDist(white.instanceID.ID, out float dist))
    //     {
    //         return dist;
    //     }
    //
    //     dist = (red.gameViewService.service.Position - white.gameViewService.service.Position).magnitude;
    //     redService.RefreshDist(white.instanceID.ID, dist);
    //     return dist;
    // }
    
    public static void SetFocusTarget(GameEntity entity, int targetId)
    {
        SetFocusTarget(entity, EntityUtils.GetGameEntity(targetId));
    }
    public static void SetFocusTarget(GameEntity entity, GameEntity target)
    {
        if (target != null)
        {
            if (entity.hasFocusEntity)
            {
                if(entity.isCamera)
                    entity.focusEntity.entity.gameViewService.service.BeFocused(false);
            }
            if(entity.isCamera)
                target.gameViewService.service.BeFocused(true);
            entity.ReplaceFocusEntity(target);
        }
        else
        {
            if (entity.hasFocusEntity)
            {
                if(entity.isCamera)
                    entity.focusEntity.entity.gameViewService.service.BeFocused(false);
                entity.RemoveFocusEntity();
            }
        }
    }

    public static bool IsNetCamera(GameEntity entity)
    {
        if (entity.hasNetAgent)
        {
            return entity.netAgent.Agent.IsCamera;
        }

        return false;
    }

    public static bool CheckMotion(GameEntity entity, int motionType)
    {
        if (entity.hasLinkMotion)
        {
            return CheckMotion(entity.linkMotion.Motion, motionType);
        }

        return false;
    }

    public static bool CheckMotion(MotionEntity motion, int motionType)
    {
        return motion.motionService.service.CheckMotionType(motionType);
    }

    public static Transform GetCharacterPart(GameEntity entity, int partType)
    {
        var gameView = entity.gameViewService.service;
        switch (partType)
        {
            case CharacterModelPart.Body:
                return gameView.Model;
            case CharacterModelPart.Head:
                return gameView.Head;
            default:
                WLogger.Error("未定义相关部位");
                return gameView.Model;
        }
    }
}
