namespace WGame.Ability.Editor
{
    using System;
    
    public enum EditorPropertyType
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
        AnimatorState,
        AnimatorParam,
        CustomProperty,
        Action,

        // for list
        List,
        GameObjectList,
    }
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EditorPropertyAttribute : Attribute
    {
        public EditorPropertyAttribute(string name, EditorPropertyType type)
        {
            PropertyName = name;
            PropertyType = type;
            Edit = true;
            Deprecated = null;
            LabelWidth = 100;
        }
        
        #region property
        public string PropertyName { get; set; }

        public EditorPropertyType PropertyType { get; set; }

        public bool Edit { get; set; }

        public string Description { get; set; }

        public string Deprecated { get; set; }

        public float LabelWidth { get; set; }

        #endregion

    }
}