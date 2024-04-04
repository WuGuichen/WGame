using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class TimeAreaType
    {
        [WLable("完美伤害区")]
        public const int PerfectDamage = 1;
        [WLable("完美伤害区2")]
        public const int PerfectDamage2 = 1 << 1;
        [WLable("完美伤害区3")]
        public const int PerfectDamage3 = 1 << 2;
        [WLable("完美伤害区4")]
        public const int PerfectDamage4 = 1 << 3;
        [WLable("破绽区")]
        public const int Weak = 1 << 4;
        [WLable("起手区")]
        public const int Beginning = 1 << 5;
        [WLable("收手区")]
        public const int Ending = 1 << 6;
    }
    public class EventSetTimeArea : IData, IEventData
    {
        public string DebugName => "完美阶段";
        
        [EditorData("区域类型", EditorDataType.TypeID, 11)]
        public int AreaType { get; set; }
        
        public void Deserialize(JsonData jd)
        {
            AreaType = JsonHelper.ReadInt(jd["Area"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer,"Area", AreaType);
            return writer;
        }

        public EventDataType EventType => EventDataType.SetTimeArea;
        
        public void Enter(EventOwner owner)
        {
            owner.SetIsInPerfectArea(AreaType, true);
            owner.SetAreaAttr(AreaType, false);
        }

        public void Duration(EventOwner owner, float deltaTime, int duration, int totalTime)
        {
        }

        public void Exit(EventOwner owner, bool isBreak)
        {
            owner.SetIsInPerfectArea(AreaType, false);
            owner.SetAreaAttr(AreaType, true);
        }

        public IEventData Clone()
        {
            return new EventSetTimeArea()
            {
                AreaType = AreaType
            };
        }
    }
}