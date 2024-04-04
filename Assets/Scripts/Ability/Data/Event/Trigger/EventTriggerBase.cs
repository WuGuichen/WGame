// using LitJson;
// using WGame.Utils;
//
// namespace WGame.Ability
// {
//     public delegate void OnTriggerEvent(TriggerContext context);
//
//     public class EventTriggerBase : IData, IEventData
//     {
//         public string DebugName { get; }
//
//         [EditorData("触发类型", EditorDataType.TypeID, 5)]
//         public int TriggerType { get; set; } = 1;
//
//         
//         public virtual void Deserialize(JsonData jd)
//         {
//             TriggerType = JsonHelper.ReadInt(jd["Trigger"]);
//         }
//
//         public virtual JsonWriter Serialize(JsonWriter writer)
//         {
//             JsonHelper.WriteProperty(ref writer,"Trigger", TriggerType);
//             return writer;
//         }
//
//         public EventDataType EventType { get; }
//         public void Enter(EventOwner owner)
//         {
//             TriggerMgr.Inst.Register(TriggerType, OnEvent);
//         }
//
//         public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
//         {
//             
//         }
//
//         public virtual void OnEvent(TriggerContext context)
//         {
//         }
//
//         public void Exit(EventOwner owner, bool isBreak)
//         {
//             TriggerMgr.Inst.Unregister(TriggerType, OnEvent);
//         }
//
//         public virtual IEventData Clone()
//         {
//             return null;
//         }
//     }
// }