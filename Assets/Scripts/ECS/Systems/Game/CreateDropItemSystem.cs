// using System.Collections;
// using System.Collections.Generic;
// using Entitas;
// using UnityEngine;
//
// public class CreateDropItemSystem : ReactiveSystem<GameEntity>
// {
//     public CreateDropItemSystem(Contexts contexts) : base(contexts.game)
//     {
//         
//     }
//
//     protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
//     {
//         return context.CreateCollector(GameMatcher.Animator);
//     }
//
//     protected override bool Filter(GameEntity entity)
//     {
//         throw new System.NotImplementedException();
//     }
//
//     protected override void Execute(List<GameEntity> entities)
//     {
//         throw new System.NotImplementedException();
//     }
// }
