namespace WGame.Ability
{
    using System;
    
    public enum EditorDataType
    {
        Bool = 1,
        Int,
        Float,
        String,
        Vector2,
        Vector3,
        Vector4,
        Color,
        Quaternion,
        Enum,
        EnumNamed,
        Object,

        GameObject,
        Lable,
        TypeID,
        MaskTypeID,

        //popup list string
        AnimationClip,
        NoticeReceiver,
        AttributeTypeID,
        BuffDataTypeID,
        BUffName,
        ActionID,
        Param,

        // for list
        List,
        GameObjectList,
    }
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EditorDataAttribute : Attribute
    {
        public EditorDataAttribute(string name, EditorDataType type, float lableWidth)
        {
            PropertyName = name;
            DataType = type;
            Edit = true;
            Deprecated = null;
            LabelWidth = lableWidth;
        }

        public EditorDataAttribute(string name, EditorDataType type, int param)
        {
            Param = param;
            PropertyName = name;
            DataType = type;
            Edit = true;
            Deprecated = null;
            LabelWidth = 60;
        }
        public EditorDataAttribute(string name, EditorDataType type)
        {
            PropertyName = name;
            DataType = type;
            Edit = true;
            Deprecated = null;
            LabelWidth = 60;
        }
        
        #region property
        public string PropertyName { get; set; }

        public EditorDataType DataType { get; set; }

        public int Param { get; set; }
        public bool Edit { get; set; }

        public string Description { get; set; }

        public string Deprecated { get; set; }

        public float LabelWidth { get; set; }

        #endregion

    }
}