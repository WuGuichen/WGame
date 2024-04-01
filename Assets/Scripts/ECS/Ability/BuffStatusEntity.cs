// using System.Collections.Generic;
//
// namespace WGame.Ability
// {
//     public class BuffStatusEntity : BuffStatus
//     {
//         #region  pool
//
//         private static Stack<BuffStatusEntity> _pool = new();
//
//         public static BuffStatusEntity Get(GameEntity entity, BuffData buffData)
//         {
//             if (_pool.Count > 0)
//             {
//                 var res = _pool.Pop();
//                 res._entity = entity;
//                 res.Initialize(new BuffManager(new BuffOwnerEntity(entity)) ,buffData);
//                 return res;
//             }
//
//             return new BuffStatusEntity(entity, buffData);
//         }
//
//         private BuffStatusEntity(GameEntity entity, BuffData buffData)
//         {
//             _entity = entity;
//             Initialize(new BuffManager(new BuffOwnerEntity(entity)) ,buffData);
//         }
//
//         #endregion
//         
//         private GameEntity _entity;
//         
//         public override void OnUpdate(float deltaTime)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         protected override void OnRemove()
//         {
//             throw new System.NotImplementedException();
//         }
//     }
// }