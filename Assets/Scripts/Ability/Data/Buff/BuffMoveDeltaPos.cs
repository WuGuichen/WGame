using LitJson;
using UnityEngine;
using WGame.Utils;

namespace WGame.Ability
{
    public class BuffMoveDeltaPos : BuffData
    {
        [EditorData("移动到", EditorDataType.Vector3)]
        public Vector3 Point { get; set; }

        [EditorData("曲线", EditorDataType.Enum)]
        public WEaseType EaseType { get; set; } = WEaseType.Linear;

        public override void Deserialize(JsonData jd)
        {
            base.Deserialize(jd);
            Point = JsonHelper.ReadVector3(jd["Pos"]);
            EaseType = JsonHelper.ReadEnum<WEaseType>(jd["Ease"]);
        }

        public override JsonWriter Serialize(JsonWriter writer)
        {
            JsonHelper.WriteProperty(ref writer,"Pos", Point);
            JsonHelper.WriteProperty(ref writer,"Ease", EaseType.ToString());
            return writer;
        }
    }
}