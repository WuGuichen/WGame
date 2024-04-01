using System;

namespace WGame.Ability.Editor.Custom
{
    public class CustomDefine : StringToIDDefine
    {
        private readonly string[] stringArray;

        private readonly int[] idArray;

        public CustomDefine(Enum type)
        {
            ReflectionHelper.GetIntValueAndName(type, ref stringArray, ref idArray);
        }
        
        public CustomDefine(Type type)
        {
            ReflectionHelper.GetConstIntValueAndName(type, ref stringArray, ref idArray);
        }

        public string[] StringArray => stringArray;
        public int[] IDArray => idArray;
    }
}