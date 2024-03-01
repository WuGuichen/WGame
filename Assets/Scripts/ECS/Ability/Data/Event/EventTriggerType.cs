using UnityEngine;

namespace WGame.Ability
{
    public enum EventTriggerType
    {
        [Header("单次触发")]
        Signal = 0,
        [Header("持续触发")]
        Duration = 1,
    }
}