using UnityEngine;
using WGame.Res;

public class InteractableObjectMono : MonoBehaviour, Interactable
{
    private Transform _transform;

    private GameObject _effectKeep;
    private string _effectGet;

    private bool _isInitialized = false;

    public void Initialize(GameObject effectKeep, string effectGet)
    {
        _effectKeep = effectKeep;
        _effectGet = effectGet;
        _isInitialized = true;
    }

    public void Dispose()
    {
        if (!_isInitialized)
        {
            return;
        }
        _isInitialized = false;
        EffectMgr.DisposeEffect(_effectKeep);
        EffectMgr.LoadEffect(_effectGet, _transform.position, 4f);
        _effectKeep = null;
        ObjectPool.Inst.PushObject(gameObject);
    }

    private void Awake()
    {
        _transform = transform;
    }

    public void Interact(GameEntity entity)
    {
        WLogger.Print("恭喜获得" + entity.instanceID.ID);
        Dispose();
    }

    public Vector3 TagPos => transform.position;
    public int UID { get; }
}
