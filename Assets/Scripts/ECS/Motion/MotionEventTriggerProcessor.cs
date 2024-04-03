// using Motion;
// using WGame.Trigger;
//
// public class MotionEventTriggerProcessor
// {
//     private float elapsedTime;
//     private int curNodeIndex;
//     private bool[] addTriggerList = new bool[8];
//     private bool[] removeTriggerList = new bool[8];
//     // private List<WTrigger> triggers = new List<WTrigger>();
//     private WTrigger[] triggers = new WTrigger[8];
//     // private MotionAnimTriggerProcessor triggerProcessor;
//     private GameEntity _character;
//     
//     public MotionEventTriggerProcessor(IMotionService service, GameEntity character)
//     {
//         // triggerProcessor = service.AnimTriggerProcessor;
//         _character = character;
//     }
//
//     public void ResetState()
//     {
//         for (int i = 0; i < addTriggerList.Length; i++)
//             addTriggerList[i] = false;
//         for (int i = 0; i < removeTriggerList.Length; i++)
//             removeTriggerList[i] = false;
//     }
//
//     public void OnUpdate(float _elapsedTime)
//     {
//         elapsedTime = _elapsedTime;
//         curNodeIndex = 0;
//     }
//
//     public void Process(EventTriggerNode node)
//     {
//         if (!node.active)
//             return;
//         // localmotion的处理
//         if (node.timeEnd < 0.001f)
//         {
//             if (triggers[curNodeIndex] == null)
//             {
//                 var trigger = WTrigger.Get(node.eventType, context => context.pInt == _character.entityID.id);
//                 var types = node.triggerType;
//                 var len = node.triggerType.Length;
//                 var parameters = node.triggerParam;
//                 trigger.onTrigger.Add(() =>
//                 {
//                     for (int i = 0; i < len; i++)
//                     {
//                         triggerProcessor.DoTrigger(types[i], parameters[i]);
//                     }
//                 });
//                 WTriggerMgr.Inst.Register(trigger);
//                 triggers[curNodeIndex] = trigger;
//             }
//         }
//         else
//         {
//             if (addTriggerList[curNodeIndex])
//             {
//                 if (removeTriggerList[curNodeIndex])
//                     return;
//                 if (elapsedTime >= (node.timeEnd + node.time))
//                 {
//                     // 清除
//                     removeTriggerList[curNodeIndex] = true;
//                     var trigger = triggers[curNodeIndex];
//                     if (trigger != null)
//                     {
//                         WTriggerMgr.Inst.Cancel(trigger);
//                         triggers[curNodeIndex] = null;
//                     }
//                 }
//
//                 return;
//             }
//
//             if (elapsedTime >= node.time)
//             {
//                 addTriggerList[curNodeIndex] = true;
//                 WTrigger trigger = null;
//                 if (node.eventType.subTypeParam == 0)
//                 {
//                     trigger = WTrigger.Get(node.eventType, context => context.pInt == _character.entityID.id);
//                 }
//                 else
//                 {
//                     trigger = WTrigger.Get(node.eventType, null);
//                 }
//                 var types = node.triggerType;
//                 var len = node.triggerType.Length;
//                 var parameters = node.triggerParam;
//                 trigger.onTrigger.Add(() =>
//                 {
//                     for (int i = 0; i < len; i++)
//                     {
//                         triggerProcessor.DoTrigger(types[i], parameters[i]);
//                     }
//                 });
//                 WTriggerMgr.Inst.Register(trigger);
//                 if (triggers[curNodeIndex] != null)
//                     WLogger.Error("数据出错");
//                 triggers[curNodeIndex] = trigger;
//             }
//         }
//
//         curNodeIndex++;
//     }
//
//     public void OnMotionEnd()
//     {
//         for (int i = 0; i < triggers.Length; i++)
//         {
//             if (triggers[i] != null)
//             {
//                 WTriggerMgr.Inst.Cancel(triggers[i]);
//                 triggers[i] = null;
//             }
//         }
//     }
// }
