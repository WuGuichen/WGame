// using UnityEngine;
//
// public static class AnimType
// {
//     public static readonly int Attack1 = Animator.StringToHash("Base Layer.Attack.Attack1");
//     public static readonly int Attack2 = Animator.StringToHash("Base Layer.Attack.Attack2");
//     public static readonly int Attack3 = Animator.StringToHash("Base Layer.Attack.Attack3");
//     public static readonly int JumpAttack1 = Animator.StringToHash("Base Layer.Attack.JumpAttack1");
//     public static readonly int LocalMotion = Animator.StringToHash("Base Layer.LocalMotion");
//     public static readonly int Jump = Animator.StringToHash("Base Layer.Jump.Jump");
//     public static readonly int Step = Animator.StringToHash("Base Layer.Step.Step");
//     public static readonly int Hit_Back = Animator.StringToHash("Base Layer.Hit.Hit_Back");
//     public static readonly int Hit_Forward = Animator.StringToHash("Base Layer.Hit.Hit_Forward");
//
//     public static string HashToName(int hash)
//     {
//         var res = "未定义";
//         if (hash == Attack1)
//             res = "Attack1";
//         else if (hash == Attack2)
//             res = "Attack2";
//         else if (hash == LocalMotion)
//             res = "LocalMotion";
//         else if (hash == Jump)
//             res = "Jump";
//         else if (hash == Attack3)
//             res = "Attack3";
//         else if (hash == Step)
//             res = "Step";
//         else if (hash == JumpAttack1)
//             res = "JumpAttack1";
//         else if (hash == Hit_Back)
//             res = "Hit_Back";
//         else if (hash == Hit_Forward)
//             res = "Hit_Forward";
//         return res;
//     }
//
//     public static bool IsAttackState(int hash) => (hash == Attack1 || hash == Attack2 || hash == Attack3);
//     public static bool IsJumpState(int hash) => (hash == Jump);
//     public static bool IsLocalMotion(int hash) => (hash == LocalMotion);
//     public static bool IsStepState(int hash) => (hash == Step);
//     public static bool IsHitState(int hash) => (hash == Hit_Back || hash == Hit_Forward);
//     public static bool IsJumpAttackState(int hash) => (hash == JumpAttack1);
// }
