using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
    [SerializeField] private CanvasGroup loadingScreen;
    // Start is called before the first frame update

    private Systems _systems;
    private Contexts _contexts;

    private void Awake()
    {
        _contexts = Contexts.sharedInstance;
        _systems = new Feature("Launcher systems");
        
        // _systems.Add(new )
    }

    void Start()
    {
        _systems.Initialize();
        SceneManager.LoadScene("Scenes/Sandbox");
    }

    // Update is called once per frame
    void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}
