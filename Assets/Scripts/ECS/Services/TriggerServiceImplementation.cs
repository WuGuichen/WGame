// using Motion;
//
// public class TriggerServiceImplementation : ITriggerService
// {
//     private readonly TriggerContext _triggerContext;
//     public TriggerServiceImplementation(Contexts contexts)
//     {
//         _triggerContext = contexts.trigger;
//     }
//     
//     public Trigger.ConditionBase GetCondition(int UID)
//     {
//         return new Trigger.ConditionAlways();
//     }
//
//     public void TriggerEvent(GameEntity entity, int type, int param1, int param1Param, int param2)
//     {
//         if (entity.hasLinkMotion)
//         {
//             entity.linkMotion.Motion.motionService.service.AddTriggerEvent(type, param1, param1Param, param2);
//         }
//     }
//
//     public TriggerEntity AddTrigger(ConditionTriggerNode node, GameEntity target)
//     { 
//         var entity = _triggerContext.CreateEntity();
//         entity.AddTriggerAction(node.actionDatas);
//         entity.AddTriggerCondition(GetCondition(node.conditionUID));
//         if(node.triggerTimes > 1)
//             entity.AddTriggerActionTime(node.triggerTimes);
//         entity.AddTriggerActionTarget(target.entityID.id);
//         return entity;
//     }
//
//     public void DoAction(GameEntity entity, int type, int param1, int param2)
//     {
//             switch (type)
//             {
//                 case ActionType.TransMotion:
//                     ActionHelper.DoTransMotionByType(entity, param1);
//                     break;
//                 default:
//                     UnityEngine.Debug.LogError("请添加行动配置");
//                     break;
//             }
//     }
// }
