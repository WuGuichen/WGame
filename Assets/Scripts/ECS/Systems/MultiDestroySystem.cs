using System.Collections.Generic;
using Entitas;

// inherit from MultiReactiveSystem using the IDestroyed interface defined above
public class MultiDestroySystem : MultiReactiveSystem<IDestroyableEntity, Contexts>
{
    private readonly IFactoryService _factory;
    // base class takes in all contexts, not just one as in normal ReactiveSystems
    public MultiDestroySystem(Contexts contexts) : base(contexts)
    {
        _factory = contexts.meta.factoryService.instance;
    }

    // return an ICollector[] with a collector from each context
    protected override ICollector[] GetTrigger(Contexts contexts)
    {
        return new ICollector[] {
            contexts.game.CreateCollector(GameMatcher.Destroyed),
            contexts.input.CreateCollector(InputMatcher.Destroyed),
            contexts.weapon.CreateCollector(WeaponMatcher.Destroyed),
            contexts.sensor.CreateCollector(SensorMatcher.Destroyed),
        };
    }

    protected override bool Filter(IDestroyableEntity entity)
    {
        return entity.isDestroyed;
    }

    protected override void Execute(List<IDestroyableEntity> entities)
    {
        foreach (var e in entities)
        {
            // Debug.Log("Destroyed Entity from " + e.contextInfo.name + " context");
            if (e is GameEntity entity)
            {
                if (entity.hasInstanceID)
                {
                    _factory.RemoveCharacter(entity.instanceID.ID);
                }

                if (entity.isCamera)
                {
                    ActionHelper.DoSetCharacterCameraByID(-1);
                }
                
                if (entity.hasLinkAbility)
                {
                    entity.linkAbility.Ability.Destroy();
                }

                if (entity.hasLinkMotion)
                {
                    entity.linkMotion.Motion.motionService.service.Destroy();
                    entity.linkMotion.Motion.Destroy();
                }

                if (entity.hasLinkWeapon)
                {
                    if (entity.linkWeapon.Weapon.hasWeaponWeaponView)
                    {
                        entity.linkWeapon.Weapon.weaponWeaponView.service.Destroy();
                    }
                    entity.linkWeapon.Weapon.Destroy();
                }

                if (entity.hasLinkSensor)
                {
                    entity.linkSensor.Sensor.detectorCharacterService.service.Destroy();
                    entity.linkSensor.Sensor.Destroy();
                }

                if (entity.hasLinkVM)
                {
                    entity.linkVM.VM.vMService.service.Destroy();
                    entity.linkVM.VM.Destroy();
                }

                if (entity.hasNetAgent)
                {
                    entity.netAgent.Agent.Dispose();
                }

                if (entity.hasGameViewService)
                {
                    entity.gameViewService.service.Destroy();
                }

                if (entity.hasAttribute)
                {
                    entity.attribute.value.Destroy();
                }

                if (entity.hasAiAgent)
                {
                    entity.aiAgent.service.Destroy();
                }

                if (entity.hasUIHeadPad)
                {
                    entity.uIHeadPad.service.Destroy(entity);
                }
            }
            else if (e is WeaponEntity weaponEntity)
            {
                weaponEntity.weaponWeaponView.service.Destroy(true);
            }
            else if (e is SensorEntity sensorEntity)
            {
                if (sensorEntity.hasTriggerObjectSensor)
                {
                    sensorEntity.triggerObjectSensor.obj.Dispose();
                }
            }
            e.Destroy();
        }
    }
}
