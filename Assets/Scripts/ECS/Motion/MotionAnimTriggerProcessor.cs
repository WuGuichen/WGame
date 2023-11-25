using Motion;
using UnityEngine;

public class MotionAnimTriggerProcessor
{
    private int[] lastTriggersIndex = new int[8];
    private int loopTriggerTime = -1;
    private bool loopTrigger = false;
    public bool LoopTrigger => loopTrigger;

    public float GetLoopTriggerTime()
    {
        float res = loopTriggerTime * 0.001f;
        loopTrigger = false;
        loopTriggerTime = -1;
        loopResetTrigger = false;
        loopResetAnimClip = false;
        return res;
    }

    private bool loopResetTrigger = false;
    private bool loopResetAnimClip = false;
    private int curNodeIndex;

    private float checkTime;

    private GameEntity character;
    private MotionEntity entity;
    private IMotionService motionService;
    private MotionAnimationProcessor animationProcessor;
    private EventNodeScriptableObject currentMotion;

    public MotionAnimTriggerProcessor(IMotionService motionService)
    {
        this.motionService = motionService;
        animationProcessor = this.motionService.AnimProcessor;
    }

    public void RegisterEntity()
    {
        this.character = this.motionService.LinkEntity;
        this.entity = this.character.linkMotion.Motion;
    }

    public void ResetState(EventNodeScriptableObject curMotion)
    {
        currentMotion = curMotion;
        for (var i = 0; i < lastTriggersIndex.Length; i++)
            lastTriggersIndex[i] = -1;
        loopTriggerTime = -1;
        if (character.hasKeepTargetDistance)
            character.RemoveKeepTargetDistance();
    }
    
    public void OnUpdate(float checkTime)
    {
        this.checkTime = checkTime;
        curNodeIndex = 0;
    }
    
    public void Process(TriggerAnimationNode node)
    {
        if (!node.active)
            return;
        if (loopResetTrigger)
        {
            return;
        }
        for (var i = lastTriggersIndex[curNodeIndex] + 1; i < node.triggerTime.Length; i++)
        {
            if (checkTime >= node.triggerTime[i])
            {
                var type = node.triggerType[i];
                var param = node.triggerParam[i];
                DoTrigger(type, param);
                if (loopResetTrigger)
                    break;
            }
        }
    
        curNodeIndex++;
    }

    public void DoTrigger(int type, int param)
    {
        if (type == AnimTriggerType.RootMotion)
        {
            animationProcessor.RootMotionRate = param;
        }
        else if (type == AnimTriggerType.Move)
        {
            if (param < 0)
            {
                character.isLockPlanarVec = true;
            }
            else
            {
                character.ReplaceAnimMoveMulti(param);
            }
        }
        else if (type == AnimTriggerType.Rotate)
        {
            if (param > 0)
                character.ReplaceAnimRotateMulti(param);
            else
            {
                character.isRotateInFocus = true;
                character.ReplaceAnimRotateMulti(-param);
            }
        }
        else if (type == AnimTriggerType.OpenSensor)
        {
            if (param == SensorType.RightWeapon)
            {
                if (character.hasLinkWeapon)
                {
                    var weaponService = character.linkWeapon.Weapon.weaponWeaponView.service;
                    weaponService.StartHitTargets();
                }
            }
        }
        else if (type == AnimTriggerType.CloseSensor)
        {
            if (param == SensorType.RightWeapon)
            {
                if (character.hasLinkWeapon)
                {
                    var weaponService = character.linkWeapon.Weapon.weaponWeaponView.service;
                    weaponService.EndHitTargets();
                }
            }
        }
        else if (type == AnimTriggerType.AnimSpeed)
        {
            animationProcessor.SetAnimSpeed(param * 0.01f);
        }
        else if (type == AnimTriggerType.ThrustUp)
        {
            character.rigidbodyService.service.Velocity += new Vector3(0, param * 0.01f, 0);
        }
        else if (type == AnimTriggerType.Loop)
        {
            //第一次记下起点，第二次循环
            if (loopTriggerTime < 0)
            {
                loopTriggerTime = param;
            }
            else
            {
                loopTrigger = true;
                loopResetTrigger = param > 0;
                loopResetAnimClip = param > 100;
            }
        } 
        else if (type == AnimTriggerType.TransMotionType)
        {
            motionService.TransMotionByMotionType(param);
        }
        else if (type == AnimTriggerType.SwitchMotion)
        {
            motionService.SwitchMotion(param);
        }
        else if (type == AnimTriggerType.KeepDistFromTarget)
        {
            character.ReplaceKeepTargetDistance(param*0.01f);
        }

        if (loopResetAnimClip)
        {
            for (int j = currentMotion.animationNodes.Count - 1; j >= 0; j--)
            {
                var n = currentMotion.animationNodes[j];
                var playTime = loopTriggerTime * 0.001f - n.time;
                if (playTime >= 0f)
                {
                    animationProcessor.PlayAnimationClip(n.AnimClipID, 0.2f, playTime, n.playLayer, n.isResetBaseLayer);
                    break;
                }
            }

            loopResetAnimClip = false;
        }

        if (loopResetTrigger)
        {
            for (int j = 0; j < currentMotion.triggerAnimationNodes.Count; j++)
            {
                var n = currentMotion.triggerAnimationNodes[j];
                for (int k = 0; k < n.triggerTime.Length; k++)
                {
                    if (n.triggerTime[k] >= loopTriggerTime)
                    {
                        lastTriggersIndex[j] = k - 1;
                        break;
                    }
                }
            }
        }
        else
        {
            lastTriggersIndex[curNodeIndex]++;
        }
    }
}
