using System;
using System.Collections;
using System.Reflection;
using Motion;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class MotionInspector : OdinEditorWindow
{
    private static MotionInspector _inspector;

    public static MotionInspector Instance
    {
        get
        {
            if (!_inspector)
                OpenEditor();
            return _inspector;
        }
    }

    private Object config => PrefabModel.currentConfig;

    [DisplayAsString][HideLabel]
    [Title("动作切换")] [HideIf("config")] public string tip1 = "请选择动作";

    [ShowIf("config")]
    [VerticalGroup("动作切换")]
    [LabelText("跳转设置")]
    [PropertyTooltip("不勾选则不能跳转，下面的配置不读取\n请勿增删列表元素，列表顺序即判断顺序")]
    public MyTogglePreMotion[] _togglesTrans = new MyTogglePreMotion[MotionType.Count];

    [ShowIf("config")]
    [VerticalGroup("动作切换")]
    [LabelText("打断设置")]
    [PropertyTooltip("不勾选则不能跳转，下面的配置不读取\n请勿增删列表元素，列表顺序即判断顺序")]
    public MyTogglePreMotion[] _togglesBreak = new MyTogglePreMotion[MotionType.Count];

    [ShowIf("config")]
    [VerticalGroup("动作切换")]
    [Button("保存跳转设置")]
    private void Save()
    {
        if (!PrefabModel.currentConfig)
        {
            Debug.LogError("请先加载配置");
            return;
        }

        if (PrefabModel.currentConfig.nextReaction.Length < MotionType.Count)
        {
            PrefabModel.currentConfig.nextReaction = new int[MotionType.Count];
            PrefabModel.currentConfig.transTime = new int[MotionType.Count];
        }

        if (PrefabModel.currentConfig.nextBreaking.Length < MotionType.Count)
        {
            PrefabModel.currentConfig.nextBreaking = new int[MotionType.Count];
            PrefabModel.currentConfig.breakTime = new int[MotionType.Count];
        }

        for (int i = 0; i < MotionType.Count; i++)
        {
            PrefabModel.currentConfig.nextReaction[i] = (_togglesTrans[i].Enabled ? 1 : -1) * _togglesTrans[i].mType;
            PrefabModel.currentConfig.transTime[i] = _togglesTrans[i].Dynamic;

            PrefabModel.currentConfig.nextBreaking[i] = (_togglesBreak[i].Enabled ? 1 : -1) * _togglesBreak[i].mType;
            PrefabModel.currentConfig.breakTime[i] = _togglesBreak[i].Dynamic;
        }

        Debug.Log("保存成功");
    }

    public static void OpenEditor()
    {
        _inspector = EditorWindow.GetWindow<MotionInspector>(false, "详细设置", true);
        _inspector.minSize = new Vector2(200, 300);
        _inspector.Show();
        _inspector._togglesBreak = new MyTogglePreMotion[MotionType.Count];
        _inspector._togglesTrans = new MyTogglePreMotion[MotionType.Count];
        for (var i = 0; i < MotionType.Count; i++)
        {
            _inspector._togglesTrans[i] = new MyTogglePreMotion() { mType = MotionType.EnumList[i] };
            _inspector._togglesBreak[i] = new MyTogglePreMotion() { mType = MotionType.EnumList[i] };
        }

        if (PrefabModel.currentConfig)
        {
            var nextReaction = PrefabModel.currentConfig.nextReaction;
            for (int i = 0; i < MotionType.Count; i++)
            {
                if (nextReaction.Length == i) break;
                var toggle = _inspector._togglesTrans[i];
                var reaction = Math.Abs(nextReaction[i]);
                if (reaction == 0)
                    continue;
                var transTime = PrefabModel.currentConfig.transTime[i];
                toggle = new MyTogglePreMotion() { mType = reaction, Dynamic = transTime };
                if (PrefabModel.currentConfig.nextReaction[i] > 0)
                    toggle.Enabled = true;
                _inspector._togglesTrans[i] = toggle;
            }

            var nextBreaking = PrefabModel.currentConfig.nextBreaking;
            for (int i = 0; i < MotionType.Count; i++)
            {
                if (nextBreaking.Length == i) break;
                var toggle = _inspector._togglesTrans[i];
                var breaking = Math.Abs(nextBreaking[i]);
                if (breaking == 0)
                    continue;
                var breakTime = PrefabModel.currentConfig.breakTime[i];
                toggle = new MyTogglePreMotion() { mType = breaking, Dynamic = breakTime };
                if (PrefabModel.currentConfig.nextBreaking[i] > 0)
                    toggle.Enabled = true;
                _inspector._togglesBreak[i] = toggle;
            }
        }
    }

    // private ConditionTriggerNode triggerNode;
    public void ClearAllData()
    {
    //      triggerNode = null;
    }

    [Serializable]
    public class MyTogglePreMotion
    {
        [LabelText("是否允许跳转到状态")] [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;

        private FieldInfo[] _fieldInfos;

        public string Label
        {
            get
            {
                if (_fieldInfos == null)
                {
                    Type t = typeof(MotionType);
                    _fieldInfos = t.GetFields();
                }

                foreach (var fieldInfo in _fieldInfos)
                {
                    if (fieldInfo.GetRawConstantValue().Equals(mType))
                        return fieldInfo.Name;
                }

                return
                    mType switch
                    {
                        MotionType.Attack1 => "Attack1",
                        MotionType.Attack2 => "Attack2",
                        MotionType.Attack3 => "Attack3",
                        MotionType.Jump => "Jump",
                        MotionType.LocalMotion => "LocalMotion",
                        MotionType.Step => "Step",
                        _ => "未定义"
                    };
            }
        }

        [HideInInspector] public int mType;

        [ToggleGroup("Enabled")]
        [LabelText("时间配置")]
        [PropertyRange(0, "Max"), PropertyOrder(3)]
        [SuffixLabel("ms", Overlay = true)]
        public int Dynamic = 0;

        [HideInInspector]
        [PropertyOrder(4)]
        public float Max
        {
            get
            {
                if (PrefabModel.currentConfig)
                    return PrefabModel.currentConfig.maxTime * 1000;
                return 0;
            }
        }
    }
}