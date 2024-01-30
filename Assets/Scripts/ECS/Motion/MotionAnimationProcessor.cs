using Animancer;
using Motion;
using UnityEngine;

public class MotionAnimationProcessor : AnimancerComponent
{
    #region 状态参数
    public float AnimSpeed => Playable.Speed;

    private float checkTime = 0f;

    private float lastAnimTime = -1;
    private float rootMotionRate = 1f;

    public float RootMotionRate
    {
        get => rootMotionRate;
        set => rootMotionRate = value;
    }
    #endregion
    
    private IFactoryService _factoryService;
    
    private Transform parentTrans;
    private Transform _transform;
    private Vector3 deltaPosition = Vector3.zero;

    private ClipState[] localMotionStates;
    private MixerState<Vector2> _focusMove;
    private LinearMixerState _regularMove;

    private AnimancerLayer[] _animancerLayers;
    private const int layerCount = 3;

    // ReSharper disable Unity.PerformanceAnalysis
    public void OnInit()
    {
        _factoryService = Contexts.sharedInstance.meta.factoryService.instance;
        InitAnimLayer(AnimLayerType.Base);
        InitAnimLayer(AnimLayerType.UpperBody);
        InitAnimLayer(AnimLayerType.LowerBody);
        rootMotionRate = 0f;
        UpdateRootMotion(true);
        localMotionStates = new ClipState[LocalMotionType.Count];
        _focusMove = new CartesianMixerState();
        this._transform = transform;
        parentTrans = _transform.parent;
        // UpdateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    private void InitAnimLayer(int layerType)
    {
        if (layerType == AnimLayerType.Base)
        {
            
        }
        else
        {
            Layers[layerType].SetMask(_factoryService.GetAvatarMask(layerType));
        }
    }

    public MotionAnimationProcessor(AnimancerComponent anim, IFactoryService factoryService)
    {
    }

    public void RefreshAnimClip(int type, string clipName)
    {
        _factoryService.LoadAnimationClip(clipName, clip =>
        {
            if (localMotionStates[type] == null)
            {
                var state = new ClipState(clip);
                localMotionStates[type] = state;
                var threshold = Vector2.zero;
                if (type == LocalMotionType.Idle)
                    threshold = new Vector2(0, 0);
                else if (type == LocalMotionType.Walk_F)
                    threshold = new Vector2(0, 1);
                else if (type == LocalMotionType.Run_F)
                    threshold = new Vector2(0, 2);
                bool needReset = _focusMove.ChildCount == 0;
                _focusMove.Add(state, threshold);
                if (needReset)
                {
                    ResetState(true);
                }
            }
            else
            {
                localMotionStates[type].Clip = clip;
            }
        });
    }
    
    public void RefreshAnimClip(int type, int clipId)
    {
        var clip = _factoryService.GetAnimationClip(clipId);
        if (clip == null)
        {
            WLogger.Error("无效的动画ID: " +clipId);
            return;
        }

        if (localMotionStates[type] == null)
        {
            var state = new ClipState(clip);
            localMotionStates[type] = state;
            var threshold = Vector2.zero;
            if (type == LocalMotionType.Idle)
                threshold = new Vector2(0, 0);
            else if (type == LocalMotionType.Walk_F)
                threshold = new Vector2(0, 1);
            else if (type == LocalMotionType.Run_F)
                threshold = new Vector2(0, 2);
            bool needReset = _focusMove.ChildCount == 0;
            _focusMove.Add(state, threshold);
            if (needReset)
            {
                ResetState(true);
            }
        }
        else
            localMotionStates[type].Clip = clip;
    }

    public void OnUpdate(float checkTime)
    {
        this.checkTime = checkTime;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ResetState(bool setLocalMotion)
    {
        if (_focusMove.ChildCount > 0)
        {
            rootMotionRate = 0f;
            lastAnimTime = -1f;
            if (setLocalMotion)
            {
                SetAnimLayerWeight(AnimLayerType.UpperBody, 0);
                SetAnimLayerWeight(AnimLayerType.LowerBody, 0);
                Play(AnimLayerType.Base, _focusMove, 0.25f, FadeMode.FixedDuration);
            }
        }
    }

    private AnimancerState Play(int layer, AnimancerState state, float fadeDuration = 0.25f,
        FadeMode mode = FadeMode.FromStart)
    {
        return Layers[layer].Play(state, fadeDuration, mode);
    }
    
    private AnimancerState Play(int layer, AnimationClip clip, float fadeDuration = 0.25f,
        FadeMode mode = FadeMode.FromStart, float startTime = 0f)
    {
        if (startTime > 0)
        {
            var state = Layers[layer].Play(clip, fadeDuration, mode);
            state.Time = startTime;
            return state;
        }
        else
            return Layers[layer].Play(clip, fadeDuration, mode);
    }
    
    public void ProcessAnimationNode(PlayAnimationNode node)
    {
        if (!node.active)
            return;
        if (checkTime*0.001f >= node.time + node.playTime && node.time + node.playTime > lastAnimTime)
        {
            lastAnimTime = checkTime*0.001f;
            InternalPlayAnimationClip(node.AnimClipID, node.playTransTime, node.playTime, node.playLayer, node.isResetBaseLayer);
        }
    }

    public void PlayAnimationClip(int clipID, float transTime = 0.1f, float offsetTime = 0f,
        int layer = AnimLayerType.Base, bool resetBaseLayer = false)
    {
        InternalPlayAnimationClip(clipID, transTime, offsetTime, layer, resetBaseLayer);
    }
    
    private void InternalPlayAnimationClip(int clipID, float transTime = 0.1f, float offsetTime = 0f, int layer = AnimLayerType.Base, bool resetBaseLayer = false)
    {
        var clip = _factoryService.GetAnimationClip(clipID);
        if (layer != AnimLayerType.Base && resetBaseLayer)
        {
            Play(AnimLayerType.Base, _focusMove, 0.25f, FadeMode.FixedDuration);
        }
        Play(layer, clip, transTime, FadeMode.FromStart, offsetTime);
    }
    
    private void SetAnimLayerWeight(int layer, float weight, float duration = 0.2f)
    {
        Layers[layer].StartFade(weight, duration);
    }
    
    public void UpdateMoveSpeed(float forward, float right)
    {
        _focusMove.Parameter = new Vector2(right, forward);
    }

    public void OnUpdateAnimator()
    {
        if (rootMotionRate > 0)
        {
            deltaPosition += (((100f - rootMotionRate)*_transform.parent.transform.position + rootMotionRate * Animator.deltaPosition) * 0.01f);
        }
    }
    
    public void SetAnimSpeed(float value = 1.0f)
    {
        if (value < 0)
            value = 0;
        Playable.Speed = value;
    }
    
    public void UpdateRootMotion(bool clear = false)
    {
        if (clear)
        {
            deltaPosition = Vector3.zero;
            return;
        }
        if (rootMotionRate > 0)
        {
            parentTrans.position += deltaPosition;
        }
        deltaPosition = Vector3.zero;
    }
}

public class LocalMotionType
{
    public const int Idle = 0;
    public const int Walk_F = 1;
    public const int Run_F = 2;

    public const int Count = 3;
}