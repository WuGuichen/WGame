using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityTimer;
using WGame.Res;
// using YooAsset;

public class SplashScene : MonoBehaviour
{
    [SerializeField] private GameObject bootstrapGameObject;
    // [SerializeField] private Image LogoImage;

    private Stopwatch _stopwatch = new Stopwatch();
    private bool isLoading;

    private void Awake()
    {
        // LogoImage.SetAlpha(1f);
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
            // LogoImage.SetAlpha(0);
            LoadSceneData();
        });
    }

    void OnLogoImageFadeOut()
    {
    }

    void LoadSceneData()
    {
        Timer.Register(1f, () => { SceneManager.UnloadSceneAsync("BootStrap"); });
    }

    // IEnumerator Download()
    // {
    //     int downloadingMaxNum = 10;
    //     int failedTryAgain = 3;
    //     var package = YooAssets.GetPackage("DefaultPackage");
    //     var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
    //
    //     //没有需要下载的资源
    //     if (downloader.TotalDownloadCount == 0)
    //     {
    //         yield break;
    //     }
    //
    //     //需要下载的文件总数和总大小
    //     int totalDownloadCount = downloader.TotalDownloadCount;
    //     long totalDownloadBytes = downloader.TotalDownloadBytes;
    //
    //     //注册回调方法
    //     downloader.OnDownloadErrorCallback = (fileName, error) =>
    //     {
    //         
    //     };
    //     downloader.OnDownloadProgressCallback = ((count, downloadCount, bytes, downloadBytes) =>
    //     {
    //         
    //     });
    //     downloader.OnDownloadOverCallback = succeed =>
    //     {
    //         Timer.Register(1f, () =>
    //         {
    //             SceneManager.UnloadSceneAsync("BootStrap");
    //             WLogger.Print("下载成功");
    //         });
    //     };
    //     downloader.OnStartDownloadFileCallback = (fileName, bytes) =>
    //     {
    //         
    //     };
    //
    //     //开启下载
    //     downloader.BeginDownload();
    //     yield return downloader;
    //
    //     //检测下载结果
    //     // if (downloader.Status == EOperationStatus.Succeed)
    //     // {
    //     //     //下载成功
    //     // }
    //     // else
    //     // {
    //     //     //下载失败
    //     // }
    // }
}
