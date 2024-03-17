using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string firstSceneName;
    [SerializeField] private GameObject window;

    public static bool isQuitting { get; private set; }

    private void Awake()
    {
        Application.quitting += delegate { isQuitting = true; };
        DoAwake();
    }

    void DoAwake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameSceneMgr.Inst.LoadNewScene(firstSceneName);
        // 加载更新页面
        // var go = Resources.Load<GameObject>("PatchWindow");
        // GameObject.Instantiate(go);
        window.SetActive(true);
    }

}
