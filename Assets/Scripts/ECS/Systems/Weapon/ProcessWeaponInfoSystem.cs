// using System.Collections.Generic;
// using Entitas;
// using UnityEngine;
// using Weapon;
// using WGame.Res;
//
// public class ProcessWeaponInfoSystem : ReactiveSystem<WeaponEntity>
// {
//     public ProcessWeaponInfoSystem(Contexts context) : base(context.weapon)
//     {
//     }
//
//     protected override ICollector<WeaponEntity> GetTrigger(IContext<WeaponEntity> context)
//     {
//         return context.CreateCollector(WeaponMatcher.WeaponInfo);
//     }
//
//     protected override bool Filter(WeaponEntity entity)
//     {
//         return true;
//     }
//
//     protected override void Execute(List<WeaponEntity> entities)
//     {
//         entities.ForEach(DoUpdateInfo);
//     }
//
//     private static void DoUpdateInfo(WeaponEntity weapon)
//     {
//         var info = weapon.weaponInfo.info;
//         if (info.isNewID)
//         {
//             if (weapon.hasWeaponWeaponView)
//             {
//                 weapon.weaponWeaponView.service.Push();
//                 weapon.RemoveWeaponWeaponView();
//             }
//
//             Transform parent = null;
//             if (weapon.hasLinkCharacter)
//             {
//                 parent = weapon.linkCharacter.Character.weaponService.service.WeaponHandle;
//             }
//             else
//             {
//                 parent = GameSceneMgr.Inst.itemRoot;
//             }
//
//             WeaponMgr.Inst.GetWeaponObj(weapon.weaponObject.objId, o =>
//             {
//                 weapon.ReplaceWeaponWeaponView(o.GetComponent<IWeaponViewService>());
//                 weapon.weaponWeaponView.service.SetDropState(ref info);
//             }, parent);
//         }
//         else
//         {
//             weapon.weaponWeaponView.service.SetDropState(ref info);
//         }
//     }
// }
