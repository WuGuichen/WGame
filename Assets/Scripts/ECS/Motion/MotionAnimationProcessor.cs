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
    public Vector2 MoveParam => _focusMove.Parameter;
    private LinearMixerState _regularMove;

    private AnimancerLayer[] _animancerLayers;

    // ReSharper disable Unity.PerformanceAnalysis
    private bool isInitted = false;
    public void OnInit()
    {
        if (isInitted)
        {
            return;
        }
        _factoryService = Contexts.sharedInstance.meta.factoryService.instance;
        InitAnimLayer(AnimLayerType.Base);
        InitAnimLayer(AnimLayerType.UpperBody);
        InitAnimLayer(AnimLayerType.LowerBody);
        rootMotionRate = 0f;
        ClearRootMotion();
        localMotionStates = new ClipState[LocalMotionType.Count];
        _focusMove = new CartesianMixerState();
        this._transform = transform;
        parentTrans = _transform.parent;
        // UpdateMode = AnimatorUpdateMode.AnimatePhysics;

        isInitted = true;
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

    public void RefreshAnimClip(int type, AnimationClip clip)
    {
        if (localMotionStates[type] == null)
        {
            var state = new ClipState(clip);
            localMotionStates[type] = state;
            var threshold = GetThreshold(type);
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
    }

    public void RefreshAnimClip(int type, string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            return;
        }
        _factoryService.LoadAnimationClip(clipName, clip =>
        {
            RefreshAnimClip(type, clip);
        });
    }

    private Vector2 GetThreshold(int type)
    {
        return type switch
        {
            LocalMotionType.Idle => new Vector2(0, 0),
            LocalMotionType.Walk_F => new Vector2(0, 1),
            LocalMotionType.Run_F => new Vector2(0, 2),
            LocalMotionType.Walk_B => new Vector2(0, -1),
            LocalMotionType.Run_B => new Vector2(0, -2),
            LocalMotionType.Walk_L => new Vector2(-1, 0),
            LocalMotionType.Run_L => new Vector2(-2, 0),
            LocalMotionType.Walk_R => new Vector2(1, 0),
            LocalMotionType.Run_R => new Vector2(2, 0),
            _ => Vector2.zero
        };
    }

    public void RefreshAnimClip(int type, int clipId)
    {
        var clip = _factoryService.GetAnimationClip(clipId);
        if (clip == null)
        {
            WLogger.Error("无效的动画ID: " +clipId);
            return;
        }

        RefreshAnimClip(type, clip);
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
                SetToBaseLayer();
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

        if (layer == AnimLayerType.Base)
        {
            SetToBaseLayer();
        }
        Play(layer, clip, transTime, FadeMode.FromStart, offsetTime);
    }

    private void SetToBaseLayer()
    {
        Layers[AnimLayerType.LowerBody].StartFade(0);
        Layers[AnimLayerType.UpperBody].StartFade(0);
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

    public Vector3 DeltaRootMotionPos => deltaPosition;

    public void ClearRootMotion()
    {
        deltaPosition = Vector3.zero;
    }
}

public class LocalMotionType
{
    public const int Idle = 0;
    public const int Walk_F = 1;
    public const int Run_F = 2;
    public const int Walk_B = 3;
    public const int Run_B = 4;
    public const int Walk_L = 5;
    public const int Run_L = 6;
    public const int Walk_R = 7;
    public const int Run_R = 8;

    public const int Count = 9;
}