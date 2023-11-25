// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.InteropServices.ComTypes;
// using UnityEngine;
//
// public class ProcessCharacterStateSystems : Feature
// {
//     public ProcessCharacterStateSystems(Contexts contexts)
//     {
// 		// 准备
// 		Add(new ProcessSignalSystem(contexts));
// 		Add(new ProcessPrestateSystem(contexts));
// 		
// 		// 状态结束
// 		Add(new ProcessLocalMotionEndSystem(contexts));
// 		Add(new ProcessAttackEndStateSystem(contexts));
// 		Add(new ProcessJumpEndStateSystem(contexts));
// 		Add(new ProcessJumpAttackEndSystem(contexts));
// 		Add(new ProcessStepEndSystem(contexts));
// 		
// 		// 状态开始
// 		Add(new ProcessLocalMotionStartSystem(contexts));
// 		Add(new ProcessAttackStartSystem(contexts));
// 		Add(new ProcessJumpStartSystem(contexts));
// 		Add(new ProcessJumpAttackStartSystem(contexts));
// 		Add(new ProcessStepStartSystem(contexts));
// 		
// 		
// 		Add(new OnStateChangeSystem(contexts));
// 		Add(new AnimMoveSystem(contexts));
//     }
// }
