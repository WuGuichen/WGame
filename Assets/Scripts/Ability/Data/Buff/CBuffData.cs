using System.Collections.Generic;
using LitJson;
using WGame.Utils;

namespace WGame.Ability
{
    public class CBuffData : BuffData
    {
        [EditorData("添加BUFF列表", EditorDataType.List, 86)]
        public List<string> AddBuffList { get; set; } = new();
        [EditorData("移除BUFF列表", EditorDataType.List, 86)]
        public List<string> RemoveBuffList { get; set; } = new();

        public List<ICondition> ConditionList { get; set; } = new();

        public override void Deserialize(JsonData jd)
        {
            base.Deserialize(jd);
            
            AddBuffList = JsonHelper.ReadListString(jd["AddBuffList"]);
            RemoveBuffList = JsonHelper.ReadListString(jd["RemoveBuffList"]);
            
            var jdConditions = jd["Conditions"];
            for (int i = 0; i < jdConditions.Count; ++i)
            {
                ConditionType eit = JsonHelper.ReadEnum<ConditionType>(jdConditions[i]["ConditionType"]);
                ICondition cond = Add(eit);
                var p = cond as IData;
                p.Deserialize(jdConditions[i]);
            }
        }

        public override JsonWriter Serialize(JsonWriter writer)
        {
            base.Serialize(writer);
            
            JsonHelper.WriteProperty(ref writer, "AddBuffList", AddBuffList);
            JsonHelper.WriteProperty(ref writer, "RemoveBuffList", RemoveBuffList);

            writer.WritePropertyName("Conditions");
            writer.WriteArrayStart();
            using (List<ICondition>.Enumerator itr = ConditionList.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    writer.WriteObjectStart();
                    JsonHelper.WriteProperty(ref writer, "ConditionType", itr.Current.CondType.ToString());
                    var p = itr.Current as IData;
                    writer = p.Serialize(writer);
                    writer.WriteObjectEnd();
                }
            }
            writer.WriteArrayEnd();

            return writer;
        }

        public ICondition Add(ConditionType t)
        {
            ICondition cond = null;
            switch (t)
            {
                case ConditionType.CheckHP:
                    cond = new CheckConditionAttr();
                    break;
                case ConditionType.OnBeHit:
                    cond = new CheckConditionBeHit();
                    break;
                default:
                    cond = ConditionAlways.Instance;
                    break;
            }

            ConditionList.Add(cond);

            return cond;
        }

        public void Remove(ICondition con)
        {
            ConditionList.Remove(con);
        }
        
        public void RemoveAll()
        {
            ConditionList.Clear();
        }
    }
}