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
        var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline, "Package");
        initParameters.SimulateManifestFilePath = simulateManifestFilePath;
#else
        string defaultHostServer = "http://192.168.2.156/CDN/v1.0/Android";
        string fallbackHostServer = "http://192.168.2.156/CDN/v1.0/Android";
        var initParameters = new HostPlayModeParameters();
        initParameters.BuildinQueryServices = new GameQueryServices();
        initParameters.DecryptionServices = new FileStreamDecryption();
        initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
#endif

        return package.InitializeAsync(initParameters).Task;
    }
    #endregion
    #region HybridCLR
    private void LoadMetadata()
    {
        var ta = GameResHelper.LoadTextAssetSync($"{scriptsPath}AOT_DLL_LIST");
        string[] aotDllList = ta.text.Split('|');
        foreach (var aotDllName in aotDllList)
        {
            ta = GameResHelper.LoadTextAssetSync($"{scriptsPath}{aotDllName}");
            var code = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(ta.bytes, HybridCLR.HomologousImageMode.SuperSet);
        }
    }

    private Task ScriptSettings()
    {
#if !UNITY_EDITOR
        LoadMetadata();
        var ta = GameResHelper.LoadTextAssetSync($"{scriptsPath}Goblin.dll");
        Assembly ass = Assembly.Load(ta.bytes);
#else
        Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Goblin");
#endif
        var export = ass.GetType("Goblin.Core.Export");
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
        await ScriptSettings();
        await GameResHelper.UpdateRes();
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