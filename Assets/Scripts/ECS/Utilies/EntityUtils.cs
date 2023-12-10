using BaseData.Character;
using UnityEngine;

public class EntityUtils
{
    private static readonly int playerLayer = LayerMask.NameToLayer("Player");
    private static readonly int enemyLayer = LayerMask.NameToLayer("Enemy");
    private static readonly int enemySensorLayer = LayerMask.NameToLayer("EnemyHitSensor");
    private static readonly int playerSensorLayer = LayerMask.NameToLayer("PlayerHitSensor");

    public const int CharacterBaseID = 9000000;
    public const int WeaponBaseID = 9000000;

    private static IFactoryService _factory;

    private static IFactoryService FactoryService
    {
        get
        {
            if (_factory == null)
                _factory = Contexts.sharedInstance.meta.factoryService.instance;
            return _factory;
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
            FactoryService.SetWeaponDrop(entity.linkWeapon.Weapon, entity.gameViewService.service.Position + entity.gameViewService.service.Model.forward, Quaternion.identity, Vector3.one);
            entity.linkMotion.Motion.motionService.service.ResetMotion();
        }
    }

    public static Vector3 GetCameraCharacterPos()
    {
        var entity = Contexts.sharedInstance.game.GetEntityWithEntityID(ActionHelper.CurCameraEntityID);
        if (entity != null && entity.isEnabled)
        {
            return entity.gameViewService.service.Position;
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
        var factory = Contexts.sharedInstance.meta.factoryService.instance;
        int weaponId = 1;
        factory.GenWeaponEntity(weaponId, out var weapon);
        factory.SetWeaponDrop(weapon, GetRandomPositionAroundCharacter(), Quaternion.identity, Vector3.one);
    }

    public static int GetTargetSensorLayer(GameEntity entity)
    {
        if (entity.characterInfo.value.camp == Camp.Enemy)
        {
            return playerSensorLayer;
        }
        else if (entity.characterInfo.value.camp == Camp.Player)
        {
            return enemySensorLayer;
        }
        return 0;
    }

    public static int GetLayer(GameEntity entity)
    {
        if (entity.characterInfo.value.camp == Camp.Enemy)
        {
            return enemyLayer;
        }
        else if (entity.characterInfo.value.camp == Camp.Player)
        {
            return playerLayer;
        }
        return 0;
    }
    
    public static int GetSensorLayer(GameEntity entity)
    {
        if (entity.characterInfo.value.camp == Camp.Enemy)
        {
            return enemySensorLayer;
        }
        else if (entity.characterInfo.value.camp == Camp.Player)
        {
            return playerSensorLayer;
        }
        return 0;
    }

    public static void Dispose()
    {
        _factory = null;
    }
}
