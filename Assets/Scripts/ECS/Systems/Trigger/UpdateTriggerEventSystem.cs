// using Entitas;
//
// public class UpdateTriggerEventSystem : IExecuteSystem
// {
//     private readonly IGroup<TriggerEntity> checkConditionTriggers;
//     private readonly GameContext _gameContext;
//     
//     public UpdateTriggerEventSystem(Contexts contexts)
//     {
//         checkConditionTriggers = contexts.trigger.GetGroup(TriggerMatcher.TriggerCondition);
//         _gameContext = contexts.game;
//     }
//
//     public void Execute()
//     {
//         foreach (var entity in checkConditionTriggers)
//         {
//             GameEntity target = null;
//             if (entity.hasTriggerConditionTarget)
//             {
//                 target = _gameContext.GetEntityWithEntityID(entity.triggerConditionTarget.UID);
//             }
//
//             if (entity.triggerCondition.value.Evaluate(target, 0, 0))
//             {
//                 entity.isActionTrigger = true;
//             }
//         }
//     }
// }
