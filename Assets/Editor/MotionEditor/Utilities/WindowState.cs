using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = System.Object;
namespace Motion
{
    class WindowState : IDisposable
    {
        public List<NodeControl> nodeControls = new List<NodeControl>();
        public double time;
        
        public MotionEditorWindow window;
        
        public WindowState(MotionEditorWindow window)
        {
            this.window = window;
            
        }
        
        public List<Manipulator> captured { get; } = new List<Manipulator>();
        
        public void AddCaptured(Manipulator manipulator)
        {
            if(!captured.Contains(manipulator))
                captured.Add(manipulator);
        }
        
        public void RemoveCaptured(Manipulator manipulator)
        {
            captured.Remove(manipulator);
        }

        public double GetSnappedTimeAtMousePosition(Vector2 mousePosition)
        {
            return ScreenSpacePixelToTimeAreaTime(mousePosition.x);
        }

        public Vector2 timeAreaScale
        {
            get { return window.timeArea.scale; }
        }
        
        public Vector2 timeAreaTranslation
        {
            get { return window.timeArea.translation; }
        }

        private float TrackSpacePixelToTimeAreaTime(float p)
        {
            p -= timeAreaTranslation.x;

            if (timeAreaScale.x > 0.0f)
            {
                return p / timeAreaScale.x;
            }

            return p;
        }

        internal Rect timeAreaRect => window.TimeContent;
        
        private float ScreenSpacePixelToTimeAreaTime(float p)
        {
            p -= timeAreaRect.x;
            return TrackSpacePixelToTimeAreaTime(p);
        }

        internal float TimeToPixel(double time)
        {
            return window.timeArea.TimeToPixel((float)time, timeAreaRect);
        }

        public float maxTime
        {
            get
            {
                if (nodeControls.Count > 0)
                    return nodeControls.Select(r => r.node).Max(r => r.timeEnd);
                return 0;
            }
        }

        public int motionID => PrefabModel.currentMotionID;

        public void SetData(IList<EventNode> datas, EventNodeScriptableObject eventNodeScriptableObject)
        {
            foreach (var data in datas)
            {
                if (data is PlayAnimationNode)
                {
                    eventNodeScriptableObject.animationNodes.Add(data as PlayAnimationNode);
                }
                else if (data is TriggerAnimationNode)
                {
                    eventNodeScriptableObject.triggerAnimationNodes.Add(data as TriggerAnimationNode);
                }
                else if (data is ConditionTriggerNode)
                {
                    eventNodeScriptableObject.conditionTriggerNodes.Add(data as ConditionTriggerNode);
                }
                else if (data is EventTriggerNode)
                {
                    eventNodeScriptableObject.eventTriggerNodes.Add(data as EventTriggerNode);
                }
                else if (data is ByteCodeCommandNode)
                {
                    eventNodeScriptableObject.byteCodeCommandNodes.Add(data as ByteCodeCommandNode);
                }
                else
                {
                    eventNodeScriptableObject.eventNodes.Add(data);
                }
            }

            if (PrefabModel.currentConfig)
            {
                eventNodeScriptableObject.nextReaction = PrefabModel.currentConfig.nextReaction;
                eventNodeScriptableObject.transTime = PrefabModel.currentConfig.transTime;
                eventNodeScriptableObject.nextBreaking = PrefabModel.currentConfig.nextBreaking;
                eventNodeScriptableObject.breakTime = PrefabModel.currentConfig.breakTime;
            }

            eventNodeScriptableObject.maxTime = maxTime;
        }

        public static AnimationClip GetAnimatorClip(int clipHash, RuntimeAnimatorController controller)
        {
            var clips = controller.animationClips.ToList();
            clips.ForEach(c =>
            {
                // Debug.Log(c.name);
            });
            return clips[1];
        }
        public static AnimationClip GetAnimatorClip(string triggerName, AnimatorController controller)
        {
            if (!controller)
                return null;
            var states = controller.layers[0].stateMachine.states.ToList();

            var find = states.Exists(r => r.state.name == triggerName);
            if (find)
            {
                var r = states.Find(t => t.state.name == triggerName);
                if (r.state.motion)
                {
                    return r.state.motion as AnimationClip;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        
        
        public List<EventNode> GetData(EventNodeScriptableObject eventNodeScriptable)
        {
            List<EventNode> rets = new List<EventNode>();
            if (eventNodeScriptable)
            {
                rets.AddRange(eventNodeScriptable.eventNodes);
                rets.AddRange(eventNodeScriptable.animationNodes);
                rets.AddRange(eventNodeScriptable.triggerAnimationNodes);
                rets.AddRange(eventNodeScriptable.eventTriggerNodes);
                rets.AddRange(eventNodeScriptable.conditionTriggerNodes);
                rets.AddRange(eventNodeScriptable.byteCodeCommandNodes);
            }
            return rets;
        }
        
        private Dictionary<Type, Type> dics = new Dictionary<Type, Type>()
        {
            { typeof(PlayAnimationNode),typeof(PlayAnimationControl) },
            { typeof(EventNode),typeof(NodeControl) },
            {typeof(TriggerAnimationNode), typeof(TriggerAnimationControl)},
            {typeof(EventTriggerNode), typeof(EventTriggerControl)},
            {typeof(ByteCodeCommandNode), typeof(ByteCodeCommandControl)},
        };

        public void AddNodeControl(EventNode node)
        {
            var t = node.GetType();
            var r = dics[t];

            ConstructorInfo con = r.GetConstructor(new Type[] { typeof(WindowState) });
            var controller = con.Invoke(new Object[] { this });

            FieldInfo f = r.GetField("node");
            f.SetValue(controller, node);
            
            nodeControls.Add((NodeControl)controller);
        }

        public void RemoveNodeControl(NodeControl control)
        {
            nodeControls.Remove(control);
        }
        
        public void RemoveNodeControl(EventNode node)
        {
            var control = this.nodeControls.Find(r => r.node == node);
            nodeControls.Remove(control);
        }

        public bool HasPlayAnimationControl()
        {
            return null != nodeControls.Find(r =>
            {
                if (r is PlayAnimationControl)
                    return true;
                return false;
            });
        }
        
        public GenericMenu GetMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Play/Animation"), false, OnAddPlayAniamtionEvent);
            menu.AddItem(new GUIContent("Trigger/TriggerAnimation"), false, OnAddTriggerAnimationEvent);
            menu.AddItem(new GUIContent("Trigger/EventTrigger"), false, OnAddEventTriggerEvent);
            menu.AddItem(new GUIContent("Command/ByteCode"), false, OnAddByteCodeCommandEvent);
            menu.AddItem(new GUIContent("粘贴轨道"), false, OnPasteNodeControl);
            return menu;
        }

        protected void OnAddByteCodeCommandEvent()
        {
            AddNodeControl(new ByteCodeCommandNode((float)time));
        }

        protected void OnAddPlayAniamtionEvent()
        {
            AddNodeControl(new PlayAnimationNode((float)time));
        }

        private void OnAddEmptyEvent()
        {
            
        }

        protected void OnAddTriggerAnimationEvent()
        {
            AddNodeControl(new TriggerAnimationNode((float)time));
        }

        protected void OnAddConditionTriggerEvent()
        {
            AddNodeControl(new ConditionTriggerNode((float)time));
        }

        protected void OnAddEventTriggerEvent()
        {
            AddNodeControl(new EventTriggerNode((float)(time)));
        }

        protected void OnPasteNodeControl()
        {
            PrefabModel.PasteControl(window.state);
            Debug.Log("已粘贴");
        }
        
        public void Dispose()
        {
            nodeControls.ForEach(r => Dispose());
        }
    }
}
