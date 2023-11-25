// using System;
// using Entitas;
// using UnityEngine;
//
// public class AnimatorServiceImplamentation : MonoBehaviour, IAnimatorService, IEventListener, IActionSwitchAnimationStateListener
// {
//     private Animator anim;
//     private GameEntity entity;
//     private int curPlayAnimHash;
//     
//     private readonly int ForwardId = Animator.StringToHash("Forward");
//     private readonly int RightId = Animator.StringToHash("Right");
//     private readonly int PlaySpeed = Animator.StringToHash("PlaySpeed");
//     
//     private Vector3 animDeltaPos;
//     private AnimationClipPlayInfo clipPlayInfo = new AnimationClipPlayInfo(AnimType.LocalMotion);
//     private bool useRootMotion = false;
//     [SerializeField]
//     private float _stateTime;
//     private ITimeService _time;
//
//     private void Awake()
//     {
//         anim = GetComponent<Animator>();
//         _time = Contexts.sharedInstance.meta.timeService.instance;
//     }
//
//     void UpdateStateElapsedTime()
//     {
//         if (clipPlayInfo.UsePhysics)
//             _stateTime += _time.fixedDeltaTime;
//         else
//         {
//             _stateTime += _time.deltaTime;
//         }
//     }
//
//     private void OnAnimatorMove()
//     {
//         UpdateStateElapsedTime();
//         if (useRootMotion)
//         {
//             transform.parent.transform.position += ((1f - clipPlayInfo.RootMotionRate)*transform.parent.transform.position + clipPlayInfo.RootMotionRate * anim.deltaPosition);
//         }
//         
//         var curInfo = anim.GetCurrentAnimatorStateInfo(0);
//         if (anim.IsInTransition(0))
//         {
//             var nextHash = anim.GetNextAnimatorStateInfo(0).fullPathHash;
//             curPlayAnimHash = nextHash;
//         }
//         else
//         {
//             var curHash = curInfo.fullPathHash;
//             curPlayAnimHash = curHash;
//             AutoEndAnimState(curInfo.normalizedTime);
//         }
//     }
//
//     /// <summary>
//     /// 自动结束动画, 默认都回到localMotion
//     /// </summary>
//     /// <param name="normalizedTime"></param>
//     void AutoEndAnimState(float normalizedTime)
//     {
//         if (normalizedTime >= clipPlayInfo.EndTime && AnimType.IsLocalMotion(clipPlayInfo.ClipHash) == false)
//         {
//             SwitchAnimState();
//         }    
//     }
//     
//     /// <summary>
//     /// 每个状态的进入在各自的system处理
//     /// </summary>
//     /// <param name="hash"></param>
//     void StartStateByAnimHash(int hash)
//     {
//         ClearPreStateInput();
//         anim.CrossFadeInFixedTime(hash, 0.2f, 0, 0);
//         
//     }
//
//     private void ClearPreStateInput()
//     {
//         entity.isPrepareAttackState = false;
//         entity.isPrepareStepState = false;
//         entity.isPrepareJumpState = false;
//     }
//
//     float IAnimatorService.stateTime
//     {
//         get => _stateTime;
//         set => _stateTime = value;
//     }
//
//     public void SetForwardParam(float value)
//     {
//         anim.SetFloat(ForwardId, value);
//     }
//
//     public void SetRightParam(float value)
//     {
//         anim.SetFloat(RightId, value);
//     }
//
//     public void SetSpeed(float value)
//     {
//         anim.speed = value;
//     }
//
//     public void SetAnimatorController(AnimatorOverrideController type)
//     {
//         ClearPreStateInput();
//         SwitchAnimState();
//         anim.runtimeAnimatorController = type;
//     }
//
//     /// <summary>
//     /// 每个状态开始调用
//     /// </summary>
//     public void CleanUpEndState()
//     {
//         if (!(entity.isAttackEndState || entity.isJumpEndState || entity.isLocalMotionEndState || entity.isStepEndState || entity.isJumpAttackEndState))
//             Debug.LogError("未按规定切换状态");
//         entity.isAttackEndState = false;
//         entity.isJumpEndState = false;
//         entity.isLocalMotionEndState = false;
//         entity.isStepEndState = false;
//         entity.isJumpAttackEndState = false;
//         
//         if(entity.hasSignalAttack)
//             entity.RemoveSignalAttack();
//         if(entity.hasSignalLocalMotion)
//             entity.RemoveSignalLocalMotion();
//         if(entity.hasSignalJump)
//             entity.RemoveSignalJump();
//         if(entity.hasSignalStep)
//             entity.RemoveSignalStep();
//     }
//
//     /// <summary>
//     /// 切换动画前必须调用这个一次, 配合prepareAction实现结束当前动画并顺利切换下一动画
//     /// </summary>
//     public void SwitchAnimState()
//     {
//         if (AnimType.IsLocalMotion(curPlayAnimHash))
//         {
//             entity.isLocalMotionEndState = true;
//         }
//         else if (AnimType.IsJumpState(curPlayAnimHash))
//         {
//             entity.isJumpEndState = true;
//         }
//         else if (AnimType.IsAttackState(curPlayAnimHash))
//         {
//             entity.isAttackEndState = true;
//         }
//         else if (AnimType.IsStepState(curPlayAnimHash))
//         {
//             entity.isStepEndState = true;
//         }
//         else if (AnimType.IsHitState(curPlayAnimHash))
//         {
//         }
//         else if (AnimType.IsJumpAttackState(curPlayAnimHash))
//         {
//             entity.isJumpAttackEndState = true;
//         }
//
//         entity.isStateChanged = true;
//     }
//
//     public GameEntity LinkEntity => entity;
//
//     public void RegisterEventListener(Contexts contexts, IEntity entity)
//     {
//         this.entity = entity as GameEntity;
//         this.entity.AddActionSwitchAnimationStateListener(this);
//     }
//
//     public void OnActionSwitchAnimationState(GameEntity entity, AnimationClipPlayInfo playInfo)
//     {
//         clipPlayInfo = playInfo;
//         anim.updateMode = playInfo.UsePhysics ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal;
//         useRootMotion = playInfo.RootMotionRate > 0;
//         // 开始新状态
//         StartStateByAnimHash(playInfo.ClipHash);
//         this.entity.isStateSwitchState = false;
//         _stateTime = 0.0f;
//     }
// }
//
// public struct AnimationClipPlayInfo
// {
//     public readonly int ClipHash;
//     public readonly float RootMotionRate;
//     public readonly bool UsePhysics;
//     public readonly float EndTime;
//
//     public AnimationClipPlayInfo(int clipHash, float rootMotionRate = 0.0f, bool usePhysics = true, float endTime = 0.85f)
//     {
//         ClipHash = clipHash;
//         RootMotionRate = rootMotionRate;
//         UsePhysics = usePhysics;
//         EndTime = endTime;
//     }
// }
