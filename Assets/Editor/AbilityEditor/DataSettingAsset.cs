using UnityEngine;

namespace WGame.Ability.Editor
{
    [CreateAssetMenu(fileName = "AbilityDataSetting", menuName = "Ability/Create Data Settings", order = 1)]
    public class DataSettingAsset : ScriptableObject
    {
        [EnumLabel("事件数据类型")]
        public EventDataType _Data;
        public EventTriggerType TriggerType;
    }
}
