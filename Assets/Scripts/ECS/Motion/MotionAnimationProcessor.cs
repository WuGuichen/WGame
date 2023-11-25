using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Motion;
using UnityEngine;

public class MotionAnimationProcessor
{
    #region 必要参数
    private AnimatorOverrideController _controller;
    private Animator anim;
    private int key1;
    private int key2;
    private int key3;
    private int keyUp1;
    private int keyUp2;
    private int keyLow1;
    private int keyLow2;
    private const string key1Str = "Motion1";
    private const string key2Str = "Motion2";
    private const string key3Str = "Motion3";
    private static readonly int ForwardId = Animator.StringToHash("Forward");
    private static readonly int RightId = Animator.StringToHash("Right");
    private const string keyUpper1Str = "MotionUp1";
    private const string keyUpper2Str = "MotionUp2";
    private const string keyLower1Str = "MotionLow1";
    private const string keyLower2Str = "MotionLow2";
    private TweenerCore<float, float, FloatOptions> layerWeightTweenUpper;
    private TweenerCore<float, float, FloatOptions> layerWeightTweenLower;
    
    public static readonly int LocalMotion = Animator.StringToHash("Base Layer.LocalMotion");

    private int nextClipKey;
    private int nextUpperClipKey;
    private int nextLowerClipKey;
    #endregion 
    
    #region 状态参数
    public float AnimSpeed => anim.speed;

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
    private Transform transform;
    private Vector3 deltaPosition = Vector3.zero;

    public MotionAnimationProcessor(Animator anim, IFactoryService factoryService)
    {
        _factoryService = factoryService;
        this.transform = anim.transform;
        parentTrans = transform.parent;
        this.anim = anim;
        key1 = Animator.StringToHash("Base Layer.Motion1");
        key2 = Animator.StringToHash("Base Layer.Motion2");
        key3 = Animator.StringToHash("Base Layer.Motion3");
        keyUp1 = Animator.StringToHash("UpperBody.MotionUp1");
        keyUp2 = Animator.StringToHash("UpperBody.MotionUp2");
        nextClipKey = key1;
        nextUpperClipKey = keyUp1;
        _controller = anim.runtimeAnimatorController as AnimatorOverrideController;
    }

    public void SetControllerClip(string key, AnimationClip clip)
    {
        _controller[key] = clip;
    }

    public void OnUpdate(float checkTime)
    {
        this.checkTime = checkTime;
    }

    public void ResetState(bool setLocalMotion)
    {
        lastAnimTime = -1f;
        rootMotionRate = 0f;
        if (setLocalMotion)
        {
            SetAnimLayerWeight(AnimLayerType.UpperBody, 0);
            SetAnimLayerWeight(AnimLayerType.LowerBody, 0);
            anim.CrossFadeInFixedTime(LocalMotion, 0.2f, 0, 0);
        }
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
        if (resetBaseLayer && anim.GetCurrentAnimatorStateInfo(AnimLayerType.Base).fullPathHash != LocalMotion)
            anim.CrossFadeInFixedTime(LocalMotion, 0f, 0, 0);
        if (anim.IsInTransition(AnimLayerType.UpperBody))
        {
            anim.Play(anim.GetNextAnimatorStateInfo(AnimLayerType.UpperBody).fullPathHash);
            anim.Update(0);
        }

        if (anim.IsInTransition(AnimLayerType.LowerBody))
        {
            anim.Play(anim.GetNextAnimatorStateInfo(AnimLayerType.LowerBody).fullPathHash);
            anim.Update(0);
        }
        if (anim.IsInTransition(AnimLayerType.Base))
        {
            anim.Play(anim.GetNextAnimatorStateInfo(AnimLayerType.Base).fullPathHash);
            anim.Update(0);
        }

        int playKey = 0;
        var nextClip = _factoryService.GetAnimationClip(clipID);
        if (layer == AnimLayerType.Base)
        {
            SetAnimLayerWeight(AnimLayerType.UpperBody, 0);
            SetAnimLayerWeight(AnimLayerType.LowerBody, 0);
            if (nextClipKey == key1)
            {
                _controller[key2Str] = nextClip;
                playKey = key2;
                nextClipKey = key2;
            }
            else if (nextClipKey == key2)
            {
                _controller[key3Str] = nextClip;
                playKey = key3;
                nextClipKey = key3;
            }
            else
            {
                _controller[key1Str] = nextClip;
                playKey = key1;
                nextClipKey = key1;
            }
        }
        else if (layer == AnimLayerType.UpperBody)
        {
            SetAnimLayerWeight(AnimLayerType.UpperBody, 1);
            // SetAnimLayerWeight(AnimLayerType.LowerBody, 0);
            if (nextUpperClipKey == keyUp1)
            {
                _controller[keyUpper2Str] = nextClip;
                playKey = keyUp2;
                nextUpperClipKey = keyUp2;
            }
            else
            {
                _controller[keyUpper1Str] = nextClip;
                playKey = keyUp1;
                nextUpperClipKey = keyUp1;
            }
        }
        else if (layer == AnimLayerType.LowerBody)
        {
            SetAnimLayerWeight(AnimLayerType.LowerBody, 1);
            // SetAnimLayerWeight(AnimLayerType.UpperBody, 0);
            if (nextLowerClipKey == keyLow1)
            {
                _controller[keyLower1Str] = nextClip;
                playKey = keyLow2;
                nextLowerClipKey = keyLow2;
            }
            else
            {
                _controller[keyLower1Str] = nextClip;
                playKey = keyLow1;
                nextUpperClipKey = keyLow1;
            }
        }

        anim.CrossFadeInFixedTime(playKey, transTime, layer, offsetTime);
    }
    
    private void SetAnimLayerWeight(int layer, float weight, float duration = 0.2f)
    {
        if (layer == AnimLayerType.UpperBody)
        {
            layerWeightTweenUpper?.Kill();
            layerWeightTweenUpper = DOTween.To(() => anim.GetLayerWeight(layer)
                    , value => anim.SetLayerWeight(layer, value), weight, duration)
                .SetAutoKill(true)
                .SetEase(Ease.Linear)
                .SetTarget(this);
        }
        else if (layer == AnimLayerType.LowerBody)
        {
            layerWeightTweenLower?.Kill();
            layerWeightTweenLower = DOTween.To(() => anim.GetLayerWeight(layer)
                    , value => anim.SetLayerWeight(layer, value), weight, duration)
                .SetAutoKill(true)
                .SetEase(Ease.Linear)
                .SetTarget(this);
        }
    }
    
    public void UpdateMoveSpeed(float forward, float right)
    {
        anim.SetFloat(ForwardId, forward);
        anim.SetFloat(RightId, right);
    }

    public void OnUpdateAnimator()
    {
        if (rootMotionRate > 0)
        {
            deltaPosition += (((100f - rootMotionRate)*transform.parent.transform.position + rootMotionRate * anim.deltaPosition) * 0.01f);
        }
    }
    
    public void SetAnimSpeed(float value = 1.0f)
    {
        if (value < 0)
            value = 0;
        anim.speed = value;
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