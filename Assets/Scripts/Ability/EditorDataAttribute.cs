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
        Object,

        GameObject,

        //popup list string
        AnimationClip,
        NoticeReceiver,
        CustomProperty,
        Action,

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
        public EditorDataAttribute(string name, EditorDataType type)
        {
            PropertyName = name;
            DataType = type;
            Edit = true;
            Deprecated = null;
            LabelWidth = 100;
        }
        
        #region property
        public string PropertyName { get; set; }

        public EditorDataType DataType { get; set; }

        public bool Edit { get; set; }

        public string Description { get; set; }

        public string Deprecated { get; set; }

        public float LabelWidth { get; set; }

        #endregion

    }
}