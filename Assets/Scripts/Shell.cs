using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

/// <summary>
/// Shell/游戏入口
/// </summary>
public class Shell : MonoBehaviour
{
    private void GameSettings()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 120;
    }

    private Task GameResSettings()
    {
        YooAssets.Initialize();
        var package = YooAssets.CreatePackage("Package");
        YooAssets.SetDefaultPackage(package);
#if UNITY_EDITOR || UNITY_EDITOR_OSX
        var initParameters = new EditorSimulateModeParameters();
        var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("Package");
        initParameters.SimulateManifestFilePath = simulateManifestFilePath;
#elif YOOASSETS_OFFLINE
            var initParameters = new OfflinePlayModeParameters();
#endif
        return package.InitializeAsync(initParameters).Task;
    }

    private async void Start()
    {
        GameSettings();
        await GameResSettings();
        HotfixScript.Init();
    }

    private void Update()
    {
        HotfixScript.Tick();
    }

    private void FixedUpdate()
    {
        HotfixScript.FixedTick();
    }
}