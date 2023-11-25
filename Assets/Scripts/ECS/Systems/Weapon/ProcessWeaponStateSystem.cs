// using System.Collections.Generic;
// using Entitas;
// using TWY.GameData;
//
// public class ProcessWeaponStateSystem : ReactiveSystem<WeaponEntity>
// {
//     public ProcessWeaponStateSystem(Contexts context) : base(context.weapon)
//     {
//     }
//
//     protected override ICollector<WeaponEntity> GetTrigger(IContext<WeaponEntity> context)
//     {
//         return context.CreateCollector(WeaponMatcher.WeaponState);
//     }
//
//     protected override bool Filter(WeaponEntity entity)
//     {
//         return true;
//     }
//
//     protected override void Execute(List<WeaponEntity> entities)
//     {
//         entities.ForEach(SwitchState);
//     }
//
//     private static void SwitchState(WeaponEntity weapon)
//     {
//         int state = weapon.weaponState.state;
//         var weaponData = GameData.Tables.TbWeapon[weapon.weaponTypeID.id];
//         int oldId = weapon.weaponObject.objId;
//         int newId = oldId;
//         switch (state)
//         {
//             case WeaponState.Drop:
//                 newId = weaponData.DropId;
//                 break;
//             case WeaponState.Equiped:
//                 newId = weaponData.ObjectId;
//                 break;
//             case WeaponState.Throw:
//                 newId = weaponData.ObjectId;
//                 break;
//             default:
//                 break;
//         }
//
//         if (oldId != newId)
//         {
//             weapon.ReplaceWeaponObject(newId);
//         }
//     }
// }
