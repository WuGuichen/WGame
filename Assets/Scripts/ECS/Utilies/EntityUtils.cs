using System.Collections.Generic;
using BaseData.Character;
using TWY.Physics;
using UnityEngine;

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
        return Contexts.sharedInstance.game.GetEntityWithEntityID(ActionHelper.CurCameraEntityID);
    }

    public static void DropEntityWeapon(GameEntity entity)
    {
        if (entity.hasLinkWeapon)
        {
            FactoryService.SetWeaponDrop(entity.linkWeapon.Weapon, entity.position.value + entity.gameViewService.service.Model.forward, Quaternion.identity, Vector3.one);
            entity.linkMotion.Motion.motionService.service.ResetMotion();
        }
    }

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

    public static void GenRandomCharacter()
    {
        int num = Random.Range(1, 3);
        int charId = num;
        int infoId = num;
        FactoryService.GenCharacter(charId, infoId, GetRandomPositionAroundCharacter(), Quaternion.identity, out var entity);
    }

    public static void GenRandomWeapon()
    {
        var factory = FactoryService;
        int weaponId = 1;
        factory.GenWeaponEntity(weaponId, out var weapon);
        factory.SetWeaponDrop(weapon, GetRandomPositionAroundCharacter(), Quaternion.identity, Vector3.one);
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
}
