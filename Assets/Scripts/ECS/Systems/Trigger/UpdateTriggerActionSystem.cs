// using System.Collections.Generic;
// using Entitas;
// using UnityEngine;
//
// public class UpdateTriggerActionSystem : ReactiveSystem<TriggerEntity>
// {
//     private readonly GameContext _gameContext;
//     
//     public UpdateTriggerActionSystem(Contexts contexts) : base(contexts.trigger)
//     {
//         _gameContext = contexts.game;
//     }
//
//     protected override ICollector<TriggerEntity> GetTrigger(IContext<TriggerEntity> context)
//     {
//         return context.CreateCollector(TriggerMatcher.ActionTrigger);
//     }
//
//     protected override bool Filter(TriggerEntity entity)
//     {
//         return !entity.isTriggerHasTriggered && entity.hasTriggerAction;
//     }
//
//     protected override void Execute(List<TriggerEntity> entities)
//     {
//         entities.ForEach(OnTriggerAction);
//     }
//
//     private void OnTriggerAction(TriggerEntity entity)
//     {
//         GameEntity targetEntity = null;
//         if (entity.hasTriggerActionTarget)
//             targetEntity = _gameContext.GetEntityWithEntityID(entity.triggerActionTarget.UID);
//         var actions = entity.triggerAction.value;
//         for(var i = 0; i < actions.Length; i++)
//         {
//             _triggerService.DoAction(targetEntity, actions[i].type, actions[i].param1, 0);
//         }
//
//         entity.isActionTrigger = false;
//         if (entity.hasTriggerActionTime)
//         {
//             entity.ReplaceTriggerActionTime(entity.triggerActionTime.num - 1);
//             if (entity.triggerActionTime.num == 0)
//                 entity.isDestroyed = true;
//         }
//         else
//         {
//             entity.isDestroyed = true;
//         }
//     }
//
// }
