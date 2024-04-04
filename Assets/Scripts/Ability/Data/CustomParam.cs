using System;
using LitJson;
using UnityEngine;
using WGame.Utils;

namespace WGame.Ability
{
    public static class TAnyExtension
    {
        public static Vector2Int AsVector2Int(this TAny t)
        {
            return (t as TAnyVector2Int).value;
        }
        public static Vector4 AsVector4(this TAny t)
        {
            return (t as TAnyVector4).value;
        }
    }

    public class CustomParam : IData
    {
        public string DebugName => "参数";

        [SerializeField] private DataType paramType = DataType.Bool;

        [EditorData("", EditorDataType.Param, 0)]
        public DataType ParamType
        {
            get => paramType;
            set
            {
                if (value != paramType)
                {
                    switch (value)
                    {
                        case DataType.Long:
                        case DataType.ULong:
                        case DataType.Vector2Int:
                        case DataType.Vector3Int:
                        // case DataType.Vector4:
                            WLogger.Error("暂不支持该类型");
                            return;
                        case DataType.Quaternion:
                            Value = TAny.New(DataType.Vector4);
                            break;
                        default:
                            Value = TAny.New(value);
                            break;
                    }
                }
                paramType = value;
            }
        }

        public TAny Value { get; set; } = TAny.New(DataType.Bool);

        public void Deserialize(JsonData jd)
        {
            ParamType = JsonHelper.ReadEnum<DataType>(jd["Type"]);
            var valueData = jd["Value"];
            switch (ParamType)
            {
                case DataType.Bool:
                    Value = new TAnyBool(JsonHelper.ReadBool(valueData));
                    break;
                case DataType.Int:
                    Value = new TAnyInt(JsonHelper.ReadInt(valueData));
                    break;
                case DataType.Long:
                case DataType.ULong:
                    break;
                case DataType.Float:
                    Value = new TAnyFloat(JsonHelper.ReadFloat(valueData));
                    break;
                case DataType.String:
                    Value = new TAnyString(JsonHelper.ReadString(valueData));
                    break;
                case DataType.Vector2:
                    Value = new TAnyVector2(JsonHelper.ReadVector2(valueData));
                    break;
                case DataType.Vector2Int:
                    var tmp2 = JsonHelper.ReadVector2(valueData);
                    Value = new TAnyVector2Int(new Vector2Int((int)tmp2.x, (int)tmp2.y));
                    break;
                case DataType.Vector3:
                    Value = new TAnyVector3(JsonHelper.ReadVector3(valueData));
                    break;
                case DataType.Vector3Int:
                    var tmp3 = JsonHelper.ReadVector3(valueData);
                    Value = new TAnyVector3Int(new Vector3Int((int)tmp3.x, (int)tmp3.y, (int)tmp3.z));
                    break;
                case DataType.Vector4:
                case DataType.Quaternion:
                    Value = new TAnyVector4(JsonHelper.ReadVector4(valueData));
                    break;
                    // Value = new TAnyQuaternion(JsonHelper.ReadQuaternion(valueData));
                    // break;
                case DataType.Color:
                    Value = new TAnyColor(JsonHelper.ReadColor(valueData));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public JsonWriter Serialize(JsonWriter writer)
        {
            switch (ParamType)
            {
                case DataType.Bool:
                    JsonHelper.WriteProperty(ref writer, "Value", Value.AsBool());
                    break;
                case DataType.Int:
                    JsonHelper.WriteProperty(ref writer, "Value", Value.AsInt());
                    break;
                case DataType.Long:
                    break;
                case DataType.ULong:
                    break;
                case DataType.Float:
                    JsonHelper.WriteProperty(ref writer, "Value", Value.AsFloat());
                    break;
                case DataType.String:
                    JsonHelper.WriteProperty(ref writer, "Value", Value.AsString());
                    break;
                case DataType.Vector2:
                    JsonHelper.WriteProperty(ref writer, "Value", Value.AsVector2());
                    break;
                case DataType.Vector2Int:
                    break;
                case DataType.Vector3:
                    JsonHelper.WriteProperty(ref writer, "Value", Value.AsVector3());
                    break;
                case DataType.Vector3Int:
                    break;
                case DataType.Vector4:
                case DataType.Quaternion:
                    JsonHelper.WriteProperty(ref writer, "Value", Value.AsVector4());
                    break;
                    // JsonHelper.WriteProperty(ref writer, "Value", Value.AsVector4());
                    // break;
                case DataType.Color:
                    JsonHelper.WriteProperty(ref writer, "Value", Value.AsColor());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            JsonHelper.WriteProperty(ref writer, "Type", ParamType.ToString());
            return writer;
        }

        public CustomParam Clone()
        {
            WLogger.Error("无法复制");
            return null;
        }
    }
}