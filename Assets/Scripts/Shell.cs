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
    private static MethodInfo tickFunc;
    private static MethodInfo fixedTickFunc;
    private static string scriptsPath = "Assets/GameRawRes/Scripts/";

    #region YooAssets
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
    #endregion

    #region HybridCLR
    private void LoadMetadata()
    {
        var handle = YooAssets.LoadRawFileSync($"{scriptsPath}AOT_DLL_LIST");
        string[] aotDllList = handle.GetRawFileText().Split('|');
        handle.Release();

        foreach (var aotDllName in aotDllList)
        {
            handle = YooAssets.LoadRawFileSync($"{scriptsPath}{aotDllName}");
            var code = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(handle.GetRawFileData(), HybridCLR.HomologousImageMode.SuperSet);
            handle.Release();
        }
    }

    private Task ScriptSettings()
    {
#if !UNITY_EDITOR
        LoadMetadata();
        var handle = YooAssets.LoadRawFileSync($"{scriptsPath}Goblin.dll");
        Assembly ass = Assembly.Load(handle.GetRawFileData());
        handle.Release();
#else
        Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Goblin");
#endif
        var export = ass.GetType("Goblin.GoblinExport");
        export.GetMethod("Init").Invoke(null, null);
        tickFunc = export.GetMethod("Tick");
        fixedTickFunc = export.GetMethod("FixedTick");

        return Task.CompletedTask;
    }
    #endregion

    private async Task GameSettings()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 120;

        await GameResSettings();
        //await ScriptSettings();
    }

    private async void Start()
    {
        await GameSettings();
    }

    private void Update()
    {
        if (null == tickFunc) return;
        tickFunc.Invoke(null, new object[] { Time.deltaTime });
    }

    private void FixedUpdate()
    {
        if (null == fixedTickFunc) return;
        fixedTickFunc.Invoke(null, new object[] { Time.fixedDeltaTime });
    }
}