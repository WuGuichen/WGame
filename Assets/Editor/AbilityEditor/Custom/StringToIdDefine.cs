using System.Collections.Generic;
using System.Text;

namespace WGame.Ability.Editor.Custom
{
    public interface StringToIDDefine
    {
        public string[] StringArray { get; }
        public int[] IDArray { get; }

        public static NoticeDefine Notice = new NoticeDefine();
        public static CustomDefine Attribute = new CustomDefine(typeof(WGame.Attribute.WAttrType));
        public static CustomDefine BuffData = new CustomDefine(typeof(BuffDataDefine));
        public static CustomDefine Action = new CustomDefine(typeof(WActionType));
        public static CustomDefine Param = new CustomDefine(typeof(WParamType));

        public static Dictionary<int, CustomDefine> DefineDict = new()
        {
            {0, new CustomDefine(typeof(AnimLayerType))},
            {1, new CustomDefine(typeof(MotionType))},
            {2, new CustomDefine(typeof(AStateType))},
            {3, new CustomDefine(typeof(InputType))},
            {4, new CustomDefine(typeof(MoveParamType))},
            {5, new CustomDefine(typeof(TriggerEventType))},
            {6, new CustomDefine(typeof(AbilityType))},
            {7, new CustomDefine(typeof(CharacterModelPart))},
            {8, new CustomDefine(typeof(AbilityIDs))},
        };

        public static Dictionary<System.Type, CustomDefine> DefineTypeDict = new()
        {
            { typeof(EventDataType), new CustomDefine((System.Enum)EventDataType.DoAction) }
        };

        public static void VisualizeStateType(ref StringBuilder buf, int type, bool isNotMask = false)
        {
            VisualizeMaskType(ref buf, 2, type, isNotMask);
        }

        public static void VisualizeInputType(ref StringBuilder buf, int type, bool isNotMask = false)
        {
            VisualizeMaskType(ref buf, 3, type, isNotMask);
        }

        private static void VisualizeMaskType(ref StringBuilder buf, int id, int type, bool isNotMask)
        {
            if (type > 0)
            {
                var define = DefineDict[id];
                for (int i = 0; i < define.StringArray.Length; i++)
                {
                    if (((1 << i) & type) != 0)
                    {
                        buf.Append(define.StringArray[i]);
                        buf.Append(",");
                        if (isNotMask)
                        {
                            break;
                        }
                    }
                }
            }
        }
        public static void VisualizeMotionType(ref StringBuilder buf, int type, bool isNotMask = false)
        {
            VisualizeMaskType(ref buf, 1, type, isNotMask);
        }

        public int GetIndex(int id)
        {
            for (var i = 0; i < IDArray.Length; i++)
            {
                if (IDArray[i] == id)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}