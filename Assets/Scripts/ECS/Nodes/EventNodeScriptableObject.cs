using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Motion
{
    [CreateAssetMenu(fileName = "Motion", menuName = "Motion/NewConfig", order = 1)]
    public class EventNodeScriptableObject : ScriptableObject
    {
        [ReadOnly]
        public int UID;

        public float maxTime;
        public List<EventNode> eventNodes = new List<EventNode>();
        public List<PlayAnimationNode> animationNodes = new List<PlayAnimationNode>();
        public List<TriggerAnimationNode> triggerAnimationNodes = new List<TriggerAnimationNode>();
        public List<ConditionTriggerNode> conditionTriggerNodes = new List<ConditionTriggerNode>();
        public List<EventTriggerNode> eventTriggerNodes = new List<EventTriggerNode>();
        public List<ByteCodeCommandNode> byteCodeCommandNodes = new List<ByteCodeCommandNode>();

        // 可跳转motion和反应顺序(负数表示不可跳转)
        [ReadOnly]
        public int[] nextReaction = new int[MotionType.Count];
        // [ReadOnly] public int[] nextMotion = new int[MotionType.Count];
        [ReadOnly]
        public int[] transTime = new int[MotionType.Count];
        [ReadOnly]
        public int[] nextBreaking = new int[MotionType.Count];
        [ReadOnly]
        public int[] breakTime = new int[MotionType.Count];
    }
}
