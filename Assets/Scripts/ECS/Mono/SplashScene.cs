using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityTimer;

public class SplashScene : MonoBehaviour
{
    [SerializeField] private GameObject bootstrapGameObject;
    [SerializeField] private Image LogoImage;

    private Stopwatch _stopwatch = new Stopwatch();
    private bool isLoading;

    private void Awake()
    {
        LogoImage.SetAlpha(1f);
        isLoading = false;
    }

    private void Start()
    {
        _stopwatch.Start();
        bootstrapGameObject.SetActive(true);
        _stopwatch.Stop();
        var delay = Mathf.Max(0f, Mathf.Min(2f - _stopwatch.ElapsedMilliseconds, 2f));
        Timer.Register(delay, () =>
        {
            LogoImage.SetAlpha(0);
            LoadSceneData();
        });
    }

    void OnLogoImageFadeOut()
    {
        
    }

    void LoadSceneData()
    {
    }
}
