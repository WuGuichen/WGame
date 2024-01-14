using UnityEngine;

public class PreBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject splashScene;

    private void Awake()
    {
        BootstrapStart();
    }

    void BootstrapStart()
    {
        // splashScene.SetActive(true);
        // 加载更新页面
        var go = Resources.Load<GameObject>("PatchWindow");
        GameObject.Instantiate(go);
        gameObject.SetActive(false);
    }
}
