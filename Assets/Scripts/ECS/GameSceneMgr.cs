using UnityEngine;
using WGame.Runtime;

public class GameSceneMgr : Singleton<GameSceneMgr>
{
    private EnvironmentMono environment;

    public GameSceneMgr()
    {
        environment = GameObject.FindWithTag("Environment").GetComponent<EnvironmentMono>();
    }

    public Transform genItemRoot => environment.Items;
    public Transform genCharacterRoot => environment.Characters;
    public Transform editCharacterRoot => environment.CharacterRoot;
}
