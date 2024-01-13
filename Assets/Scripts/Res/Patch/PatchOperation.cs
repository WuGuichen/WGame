using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class PatchOperation : GameAsyncOperation
{
    private enum ESteps
    {
        None,
        Update,
        Done,
    }
    
    private ESteps _steps = ESteps.None;

    private readonly string _packageName;
    private readonly string _buildPipeline;
    private readonly EPlayMode _playMode;
    
    public PatchOperation(string packageName, string buildPipeline, EPlayMode playMode)
    {
        _packageName = packageName;
        _buildPipeline = buildPipeline;
        _playMode = playMode;
    }
    protected override void OnStart()
    {
        _steps = ESteps.Update;
        // 初始化资源包
        WLogger.Info("初始化资源包");
    }

    protected override void OnUpdate()
    {
        if (_steps == ESteps.None || _steps == ESteps.Done)
            return;
        if (_steps == ESteps.Update)
        {
            
        }
    }

    protected override void OnAbort()
    {
        
    }

    private IEnumerator InitPackage()
    {
        // 创建资源包
        var package = YooAssets.TryGetPackage(_packageName);
        if (package == null)
            package = YooAssets.CreatePackage(_packageName);
        
        // 编辑器模拟模式
        InitializationOperation initializationOperation = null;
        switch (_playMode)
        {
            case EPlayMode.EditorSimulateMode:
                var createParameters = new EditorSimulateModeParameters();
                // createParameters.SimulateManifestFilePath =
                //     EditorSimulateModeHelper.SimulateBuild(_buildPipeline, _packageName);
                break;
            case EPlayMode.OfflinePlayMode:
                break;
            case EPlayMode.HostPlayMode:
                break;
        }
        
        yield return null;
    }
}
