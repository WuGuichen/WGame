using UnityEngine;
using UnityEngine.SceneManagement;
using WGame.Runtime;

public class GameSceneMgr : Singleton<GameSceneMgr>
{
    private EnvironmentMono environment;
    public EnvironmentMono Environment => environment;
    
    private Transform _transform;

    private bool isInitted = false;

    public void SetEnvironment()
    {
        if (isInitted)
        {
            return;
        }

        var o = GameObject.FindGameObjectWithTag("Environment");
        environment = o.GetComponent<EnvironmentMono>();
        _transform = o.transform;
        isInitted = true;
            YooassetManager.Inst.LoadGameObject("GameSystems",
                o => { });
    }

    public Transform genItemRoot => environment.Items;
    public Transform genCharacterRoot => environment.Characters;
    public Transform editCharacterRoot => environment.CharacterRoot;
    public Transform Root => _transform;
    public Transform TriggerObjectRoot => environment.TriggerObjRoot;

    public void LoadNewScene(string sceneName)
    {
        EventCenter.AddListener(EventDefine.OnGameAssetsManagerInitted, () =>
        {
            var handle = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            handle.completed += operation =>
            {
                EventCenter.Trigger(EventDefine.OnSceneLoaded, sceneName);
            };
        });
    }

    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
