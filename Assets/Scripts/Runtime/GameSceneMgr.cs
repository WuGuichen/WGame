using UnityEngine;
using UnityEngine.SceneManagement;
using WGame.Runtime;

public class GameSceneMgr : Singleton<GameSceneMgr>
{
    private EnvironmentMono environment;

    public void InitEnvironment()
    {
        environment = GameObject.FindWithTag("Environment").GetComponent<EnvironmentMono>();
        TriggerObjectRoot = new GameObject("TriggerObjectRoot").transform;
        GameObject.DontDestroyOnLoad(TriggerObjectRoot);
    }

    public Transform genItemRoot => environment.Items;
    public Transform genCharacterRoot => environment.Characters;
    public Transform editCharacterRoot => environment.CharacterRoot;
    public Transform TriggerObjectRoot { get; private set; }

    public void LoadNewScene(string sceneName)
    {
        var handle = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        handle.completed += operation =>
        {
            EventCenter.Trigger(EventDefine.OnSceneLoaded, WEventContext.Get(sceneName));
        };
    }

    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
