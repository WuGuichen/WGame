namespace WGame.Utils
{
    using UnityEngine;
    
    public interface TAny
    {
        public static TAny New(DataType type)
        {
            TAny obj = null;
            switch (type)
            {
                case DataType.Bool:
                    obj = new TAnyBool(false);
                    break;
                case DataType.Int:
                    obj = new TAnyInt(0);
                    break;
                case DataType.Long:
                    obj = new TAnyLong(0);
                    break;
                case DataType.Float:
                    obj = new TAnyFloat(0f);
                    break;
                case DataType.String:
                    obj = new TAnyString(string.Empty);
                    break;
                case DataType.Vector2:
                    break;
                case DataType.Vector3:
                    obj = new TAnyVector3(Vector3.zero);
                    break;
                case DataType.Vector3Int:
                    obj = new TAnyVector3Int(Vector3Int.zero);
                    break;
                case DataType.Vector4:
                    obj = new TAnyVector4(Vector4.zero);
                    break;
                case DataType.Quaternion:
                    obj = new TAnyQuaternion(Quaternion.identity);
                    break;
                case DataType.Color:
                    obj = new TAnyColor(Color.black);
                    break;
            }
            return obj;
        }

        public static void Set<T>(TAny obj, T v)
        {
            var t = obj as TAnyVal<T>;
            t.value = v;
        }
    }
    
    public class TAnyVal<T> : TAny
    {
        public T value
        {
            get;
            set;
        }

        public TAnyVal(T v)
        {
            value = v;
        }
    }

    public sealed class TAnyBool : TAnyVal<bool>
    {
        public TAnyBool(bool v) : base(v)
        { }
    }

    public sealed class TAnyInt : TAnyVal<int>
    {
        public TAnyInt(int v) : base(v)
        { }
    }

    public sealed class TAnyLong : TAnyVal<long>
    {
        public TAnyLong(long v) : base(v)
        { }
    }

    public sealed class TAnyFloat : TAnyVal<float>
    {
        public TAnyFloat(float v) : base(v)
        { }
    }

    public sealed class TAnyString : TAnyVal<string>
    {
        public TAnyString(string v) : base(v)
        { }
    }

    public sealed class TAnyVector2 : TAnyVal<Vector2>
    {
        public TAnyVector2(Vector2 v) : base(v)
        { }
    }

    public sealed class TAnyVector2Int : TAnyVal<Vector2Int>
    {
        public TAnyVector2Int(Vector2Int v) : base(v)
        { }
    }

    public sealed class TAnyVector3 : TAnyVal<Vector3>
    {
        public TAnyVector3(Vector3 v) : base(v)
        { }
    }


    public sealed class TAnyVector3Int : TAnyVal<Vector3Int>
    {
        public TAnyVector3Int(Vector3Int v) : base(v)
        { }
    }

    public sealed class TAnyVector4 : TAnyVal<Vector4>
    {
        public TAnyVector4(Vector4 v) : base(v)
        { }
    }

    public sealed class TAnyQuaternion : TAnyVal<Quaternion>
    {
        public TAnyQuaternion(Quaternion v) : base(v)
        { }
    }

    public sealed class TAnyColor : TAnyVal<Color>
    {
        public TAnyColor(Color v) : base(v)
        { }
    }
}