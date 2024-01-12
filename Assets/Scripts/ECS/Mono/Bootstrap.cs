using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTimer;
using WGame.UI;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string firstSceneName;

    public static bool isQuitting { get; private set; }

    private void Awake()
    {
        Application.quitting += delegate { isQuitting = true; };
        DoAwake();
    }

    void DoAwake()
    {
        DontDestroyOnLoad(this);
        
        MainModel.Inst.IsBootByBootstrap = true;
    }

    private void Start()
    {
        var handle = SceneManager.LoadSceneAsync(firstSceneName, LoadSceneMode.Additive);
        handle.completed += operation =>
        {
            Timer.Register(1f, () =>
            {
                SceneManager.UnloadSceneAsync("BootStrap");
            });
        };
    }
}
