using System;
using System.Collections.Generic;
using BaseData.Character;
using Entitas.Unity;
#if UNITY_EDITOR
using System.IO;
#endif
using UnityEngine;
using Motion;
using Pathfinding;
using Weapon;
using WGame.Attribute;
using WGame.Res;
using WGame.Runtime;
using Random = UnityEngine.Random;

public class FactoryServiceImplementation : IFactoryService
{
    private Transform dropItemRoot;
    private Transform sceneRoot;
    
    private Dictionary<int, AnimationClip> _clips = new();
    private Dictionary<int, EventNodeScriptableObject> _motions = new Dictionary<int, EventNodeScriptableObject>();

    private readonly InstaceDB<GameEntity> _gameEntityDB;

    private const int characterBaseID = EntityUtils.CharacterBaseID;
    private int genCharacterNum = 0;
    private const int weaponBaseID = EntityUtils.WeaponBaseID;
    private int genWeaponNum = 0;

    #if UNITY_EDITOR
    private FileSystemWatcher motionWatcher;
    #endif

    private readonly GameContext _gameContext;
    private readonly WeaponContext _weaponContext;

    public FactoryServiceImplementation(Contexts contexts)
    {
#if UNITY_EDITOR
        motionWatcher = GetWatcher(Application.dataPath + "/Res/Motions/", OnMotionChanged);
#endif
        _gameContext = contexts.game;
        _weaponContext = contexts.weapon;
        _gameEntityDB = new InstaceDB<GameEntity>(EntityUtils.CharacterBaseID);
    }
    
    #if UNITY_EDITOR
    private FileSystemWatcher GetWatcher(string dir, FileSystemEventHandler handler)
    {
        var watcher = new FileSystemWatcher();
        watcher.Path = dir;
        watcher.Filter = "*.asset";
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += handler;
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
        return watcher;
    }
    #endif

    private int clipNum = 0;
    void InitClips()
    {
        var clips = GameData.Tables.TbClip.DataList;
        clipNum = clips.Count;
        clips.ForEach(c =>
        {
            YooassetManager.Inst.LoadAnimationClipAsync(c.Path, clip =>
            {
                _clips[c.Id] = clip;
                clipNum--;
                if (clipNum <= 0)
                {
                    EventCenter.Trigger(EventDefine.OnGameResourcesLoaded);
                }
            });
        });
    }
    public void InitSceneObjectRoot(Transform sceneRoot)
    {
        this.sceneRoot = sceneRoot;
        if (dropItemRoot == null)
        {
            var root = new GameObject("DropItemRoot");
            dropItemRoot = GameObject.Instantiate(root).transform;
            dropItemRoot.name = dropItemRoot.name.Replace("(Clone)", "");
        }
        dropItemRoot.SetParent(this.sceneRoot);
        InitClips();
    }

    public AnimationClip GetAnimationClip(int clipID)
    {
        return _clips[clipID];
    }

    public void LoadAnimationClip(string clipName, Action<AnimationClip> callback)
    {
        YooassetManager.Inst.LoadAnimationClipAsync(clipName, callback);
    }

    public EventNodeScriptableObject GetMotion(int motionID)
    {
        if (!_motions.TryGetValue(motionID, out var motion))
        {
            var ids = MotionIDs.IDList;
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == motionID)
                {
                    YooassetManager.Inst.LoadMotionScriptSync(MotionIDs.NameList[i], o => motion = o);
                    _motions[motionID] = motion;
                    break;
                }
            }
        }
        return motion;
    }

    public void Dispose()
    {
        #if UNITY_EDITOR
        if (motionWatcher != null)
        {
            motionWatcher.Dispose();
            motionWatcher = null;
        }
        #endif
        _gameEntityDB.Clear();
    }

#if UNITY_EDITOR
    private void OnMotionChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
    {
        int motionID = 0;
        string path = fileSystemEventArgs.Name.Split('.')[0];
        var names = MotionIDs.NameList;
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == path)
            {
                motionID = MotionIDs.IDList[i];
                YooassetManager.Inst.LoadMotionScriptAsync(path, o => _motions[motionID] = o);
                break;
            }
        }
        WLogger.Print("Motion更新" + path);
    }
#endif

    public void RemoveCharacter(int instanceID)
    {
        _gameEntityDB.Cancel(instanceID);
    }

    public void GenCharacter(int charID, int infoID, Vector3 pos, Quaternion rot, out GameEntity entity, Action<GameEntity> callback = null)
    {
        var data = GameData.Tables.TbCharacter[charID];
        if (data == null)
        {
            entity = null;
            WLogger.Error("没有相应角色id：" + charID);
            return;
        }

        int id = characterBaseID + genCharacterNum;
        genCharacterNum++;
        entity = _gameContext.CreateEntity();
        entity.AddEntityID(id);
        entity.AddCharacterInfo(WCharacterInfo.GetCharacterInfo(infoID));
        entity.AddPosition(pos);
        var gameEntity = entity;
        ObjectPool.Inst.GetObject(data.ObjectId, pos, rot, GameSceneMgr.Inst.genCharacterRoot, obj =>
        {
            obj.name = id.ToString();

            InitCharacter(ref gameEntity, obj);
            
            // 真正完成角色生成
            callback?.Invoke(gameEntity);
        });
    }

    public void GenCharacter(GameObject obj)
    {
        var info = obj.GetComponent<WCharacterInfo>().GetCharacterInfo();
        int id = int.Parse(obj.name);
        var entity = _gameContext.CreateEntity();
        entity.AddEntityID(id);
        entity.AddCharacterInfo(info);
        entity.AddPosition(obj.transform.position);
        InitCharacter(ref entity, obj);
    }

    private void InitCharacter(ref GameEntity entity, GameObject obj)
    {
        int instanceID = _gameEntityDB.Register(entity);
        entity.AddInstanceID(instanceID);
        
        var info = entity.characterInfo.value;
        // 游戏物体
        entity.AddGameViewService(obj.GetComponent<IGameViewService>().OnInit(entity));
        // 刚体
        entity.AddRigidbodyService(obj.GetComponent<IRigidbodyService>().OnInit());
        entity.rigidbodyService.service.SetEntity(entity);
        
        obj.layer = EntityUtils.GetLayer(entity);
        entity.gameViewService.service.Model.gameObject.layer = EntityUtils.GetSensorLayer(entity);
        entity.AddCharacterSensor(1 << EntityUtils.GetTargetSensorLayer(entity), 10f);

        // motion
        var motion = Contexts.sharedInstance.motion.CreateEntity();
        // 链接
        entity.AddLinkMotion(motion);
        motion.AddLinkCharacter(entity);
        motion.AddMotionService(obj.GetComponentInChildren<IMotionService>().OnInit(motion));
        motion.AddMotionLocalMotion(MotionIDs.LocalMotion1);
        motion.AddMotionStepFwd(MotionIDs.Step1);
        motion.AddMotionHitFwd(MotionIDs.Hit1);
        motion.AddMotionSpare(MotionIDs.Spare1);
        motion.AddMotionHitBwd(MotionIDs.HitBwd1);
        motion.AddMotionJump(MotionIDs.Jump1);
        motion.AddMotionJumpLand(MotionIDs.JumpLand1);
        motion.AddMotionDefense(MotionIDs.Block1);
        motion.AddMotionJumpAttack(MotionIDs.JumpAttack1);
        motion.AddMotionDead(MotionIDs.Dead_R1);
        // 初始motion
        motion.AddMotionStart(motion.motionLocalMotion.UID);
        // 武器位
        entity.AddWeaponService(obj.GetComponentInChildren<IWeaponService>());

        // 不用等待GameObject的组件的绑定
        entity.AddMovementSpeed(info.moveSpeed);
        entity.AddRotationSpeed(info.rotateSpeed);
        entity.AddRunningMultiRate(info.runMultiRate);
        entity.AddJumpForce(5f);
        entity.AddCharGravity(-10f);
        entity.AddCharCurSpeed(0f);
        entity.AddCharSpeedMulti(1f);
        entity.AddAnimRotateMulti(100);
        entity.AddAnimMoveMulti(100);
        var attribute = new WAttribute(entity);
        attribute.Set(WAttrType.MaxHP, info.MaxHP);
        attribute.Set(WAttrType.CurHP, info.CurHP);
        attribute.Set(WAttrType.MaxMP, info.MaxMP);
        attribute.Set(WAttrType.CurMP, info.CurMP);
        attribute.Set(WAttrType.ATK, info.ATK);
        attribute.Set(WAttrType.DEF, info.DEF);
        entity.AddAttribute(attribute);

        // 地面检测状态
        entity.AddGroundSensor(false);
        // 动画速度
        entity.AddAnimationSpeed(1f);
        // 是否开启掉落物检测
        entity.isDropItemSensor = true;
        // 是否可移动
        entity.isMoveable = true;

        // UI组件
        entity.AddUIHeadPad(new HeadPadUIServiceImplementation());

        // ----- LinkAbilityEntity -----
        var ability = Contexts.sharedInstance.ability.CreateEntity();
        // 受击能力
        ability.AddAbilityGotHit(new GotHitAbilityServiceImplementation());
        entity.AddLinkAbility(ability);
        ability.AddLinkCharacter(entity);
        // ----- end -----

        // 虚拟机
        var vm = Contexts.sharedInstance.vM.CreateEntity();
        vm.AddVMService(new VMServiceImplementation());
        vm.vMService.service.Set("E_SELF", entity.entityID.id);

        entity.AddLinkVM(vm);
        vm.AddLinkCharacter(entity);
        
        // AI代理控制
        entity.AddAiAgent(new AiAgentServiceImplementation(entity, obj.GetComponent<Seeker>(),
            info.patrolPoints));

        obj.Link(entity);
        
        // Sensor
        var sensor = Contexts.sharedInstance.sensor.CreateEntity();
        entity.AddLinkSensor(sensor);
        sensor.AddLinkCharacter(entity);
        sensor.AddSensorCharacterService(new SensorCharacterImplementation(entity.gameViewService.service.Model, sensor));
        sensor.AddDetectSpottedRadius(5f);
        sensor.AddDetectWarningRadius(10f);
        sensor.AddDetectCharacterDegree(120);
        sensor.AddSensorCharRadius(2f);
        sensor.isDetectCharOpen = true;
        sensor.isSensorCharOpen = true;
        

        // 事件监听
        var listeners = obj.GetComponentsInChildren<IEventListener>();
        foreach (var listener in listeners)
            listener.RegisterEventListener(Contexts.sharedInstance, entity);
        entity.uIHeadPad.service.RegisterEvent(entity.attribute.value);

        var vmService = entity.linkVM.VM.vMService.service;
        vmService.TriggerEvent(EventDefine.OnCharacterInit, new List<Symbol>() { new(entity.entityID.id) });

        // todo 空手武器
        if (info.Weapon > 0)
        {
            GenWeaponEntity(info.Weapon, out var weaponEntity);
            SetWeaponEquipTo(weaponEntity, entity);
        }
        else
        {
            motion.motionService.service.ResetMotion();
        }
        
        // 加入bvh
        if (info.camp == Camp.Red)
        {
            EntityUtils.BvhEnemy.Add(entity.gameViewService.service);
        }
    }

    public void GenWeaponEntity(int weaponID, out WeaponEntity weapon)
    {
        var data = GameData.Tables.TbWeapon[weaponID];
        if (data == null)
        {
            WLogger.Error("weapons没有相应配置id: " + weaponID);
            weapon = null;
            return;
        }
        int id = weaponBaseID + genWeaponNum;
        genWeaponNum++;
        weapon = _weaponContext.CreateEntity();
        weapon.AddEntityID(id);
        weapon.AddWeaponObject(-1);
        weapon.AddWeaponState(-1);
        weapon.AddWeaponTypeID(weaponID);
    }

    public void SetWeaponDrop(int entityID, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        var weapon = _weaponContext.GetEntityWithEntityID(entityID);
        if (weapon != null && weapon.isEnabled)
        {
            SetWeaponDrop(weapon, pos, rot, scale);
        }
    }

    public void SetWeaponDrop(WeaponEntity weapon, Vector3 pos, Quaternion rot, Vector3 scale)
    {
        int oldId = weapon.weaponObject.objId;
        int newId = oldId;
        // weapon.ReplaceWeaponState(WeaponState.Drop);
        var weaponData = GameData.Tables.TbWeapon[weapon.weaponTypeID.id];
        newId = weaponData.DropId;

        bool isNew = oldId != newId;

        var info = new WeaponInfo()
        {
            isNewID = isNew,
            position = pos,
            rotation = rot,
            scale = scale,
        };
        
        if (isNew)
        {
            weapon.ReplaceWeaponObject(newId);
            var parent = GameSceneMgr.Inst.genItemRoot;
            if (weapon.hasWeaponWeaponView)
            {
                weapon.weaponWeaponView.service.Push();
                weapon.RemoveWeaponWeaponView();
            }
            
            WeaponMgr.Inst.GetWeaponObj(weapon.weaponObject.objId, o =>
            {
                weapon.ReplaceWeaponWeaponView(o.GetComponent<IWeaponViewService>());
                if (weapon.hasLinkCharacter)
                {
                    weapon.weaponWeaponView.service.UnLinkCharacter(weapon.linkCharacter.Character);
                }
                weapon.weaponWeaponView.service.RegisterEntity(weapon);
                weapon.weaponWeaponView.service.SetDropState(ref info);
            }, parent);
        }
        else
        {
            if (weapon.hasLinkCharacter)
            {
                weapon.weaponWeaponView.service.UnLinkCharacter(weapon.linkCharacter.Character);
            }
            weapon.weaponWeaponView.service.SetDropState(ref info);
        }
    }

    public void SetWeaponEquipTo(WeaponEntity weapon, GameEntity entity)
    {
        if (entity == null || entity.isEnabled == false)
            return;
        var data = GameData.Tables.TbWeapon[weapon.weaponTypeID.id];
        if (weapon.hasWeaponWeaponView)
        {
            if (weapon.hasLinkCharacter)
            {
                // 先解除
                if (weapon.linkCharacter.Character.entityID.id == entity.entityID.id)
                {
                    // 已装备
                    return;
                }

                weapon.weaponWeaponView.service.UnLinkCharacter(weapon.linkCharacter.Character);
            }

            weapon.weaponWeaponView.service.LinkToCharacter(entity);
        }
        else
        {
            WeaponMgr.Inst.GetWeaponObj(data.ObjectId, o =>
            {
                weapon.AddWeaponWeaponView(o.GetComponent<IWeaponViewService>().RegisterEntity(weapon));
                weapon.weaponWeaponView.service.LinkToCharacter(entity);
            });
        }

        var motion = entity.linkMotion.Motion.motionService.service;
        motion.SetLocalMotion("Walk_F", data.WalkF);
        motion.SetLocalMotion("Run_F", data.RunF);
        motion.SetLocalMotion("Idle", data.Idle);
        var motionEntt = entity.linkMotion.Motion;
        motionEntt.ReplaceMotionAttack1(data.Attack1);
        motionEntt.ReplaceMotionAttack2(data.Attack2);
        motionEntt.ReplaceMotionAttack3(data.Attack3);
    }

    public GameEntity SelectRandomGenCharacter()
    {
        if (genCharacterNum <= 0)
            return null;
        for (int i = 0; i < 400; i++)
        {
            int num = Random.Range(0, genCharacterNum+1);
            var entity = _gameContext.GetEntityWithEntityID(num + characterBaseID);
            if (entity != null && entity.isEnabled && entity.entityID.id != ActionHelper.CurCameraEntityID)
            {
                return entity;
            }
        }

        return null;
    }

    public GameEntity GetGameEntity(int instId)
    {
        return _gameEntityDB[instId];
    }
}
