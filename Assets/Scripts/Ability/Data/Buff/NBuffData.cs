using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using WGame.Utils;

namespace WGame.Ability
{
    public class NBuffData : BuffData
    {
        [EditorData("属性类型", EditorDataType.AttributeTypeID)]
        public int AttrID { get; set; } = 99;
        [EditorData("属性加值", EditorDataType.Int)]
        public int AddValue { get; set; } = 0;
        [EditorData("属性乘值", EditorDataType.Int)]
        public int MulValue { get; set; } = 0;

        public override void Deserialize(JsonData jd)
        {
            base.Deserialize(jd);

            AttrID = JsonHelper.ReadInt(jd["Attr"]);
            AddValue = JsonHelper.ReadInt(jd["AddVal"]);
            MulValue = JsonHelper.ReadInt(jd["MulVal"]);
        }

        public override JsonWriter Serialize(JsonWriter writer)
        {
            base.Serialize(writer);
            
            JsonHelper.WriteProperty(ref writer, "Attr", AttrID);
            JsonHelper.WriteProperty(ref writer, "AddVal", AddValue);
            JsonHelper.WriteProperty(ref writer, "MulVal", MulValue);

            return writer;
        }
    }
}