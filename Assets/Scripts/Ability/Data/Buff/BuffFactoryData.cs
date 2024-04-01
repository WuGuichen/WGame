using LitJson;
using UnityEngine;
using WGame.Utils;

namespace WGame.Ability
{
    public class BuffFactoryData : IData
    {
        [SerializeField]
        private BuffType _buffType = BuffType.None;

        [field: SerializeReference]
        public BuffData Buff { get; set; }

        public string DebugName => Buff != null ? Buff.Name : "noname" + System.DateTime.Now.ToString("yyyyMMddhhmmss");

        public int ID => Buff != null ? Buff.ID : -1;
        public string Name => Buff != null ? Buff.Name : "None";

        #region property
        [EditorData("BUFF类型", EditorDataType.Enum)]
        public BuffType BuffType
        {
            get => _buffType;
            set
            {
                if (_buffType != value)
                {
                    _buffType = value;
                    switch (_buffType)
                    {
                        case BuffType.Numerical:
                            Buff = new NBuffData();
                            break;
                        case BuffType.Condition:
                            Buff = new CBuffData();
                            break;
                    }
                }
            }
        }
        #endregion property

        public void Deserialize(JsonData jd)
        {
            BuffType = JsonHelper.ReadEnum<BuffType>(jd["BuffType"]);
            Buff.Deserialize(jd["BuffData"]);
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            writer.WriteObjectStart();
            JsonHelper.WriteProperty(ref writer, "BuffType", BuffType.ToString());
            writer.WritePropertyName("BuffData");
            writer.WriteObjectStart();
            if (Buff == null)
            {
                Buff = new BuffData();
            }
            writer = Buff.Serialize(writer);
            writer.WriteObjectEnd();

            writer.WriteObjectEnd();
            
            return writer;
        }
    }
}