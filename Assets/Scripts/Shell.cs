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
    private MethodInfo tickFunc;
    private MethodInfo fixedTickFunc;

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
        var initParameters = new HostPlayModeParameters();
        initParameters.BuildinQueryServices = new GameQueryServices();
        initParameters.DecryptionServices = new FileStreamDecryption();
        initParameters.RemoteServices = new RemoteServices();
#endif

        return package.InitializeAsync(initParameters).Task;
    }
    #endregion

    #region HybridCLR
    private void LoadMetadata()
    {
        var ta = LoadScriptTASync("AOT_DLL_LIST");
        string[] aotDllList = ta.text.Split('|');
        foreach (var aotDllName in aotDllList)
        {
            ta = LoadScriptTASync(aotDllName);
            var code = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(ta.bytes, HybridCLR.HomologousImageMode.SuperSet);
        }
    }

    private Task ScriptSettings()
    {
#if !UNITY_EDITOR
        LoadMetadata();
        var ta = LoadScriptTASync("Goblin.dll");
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

    private async void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = int.MaxValue;

        await GameResSettings();
        await ScriptSettings();
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

    private TextAsset LoadScriptTASync(string resName)
    {
        var handle = YooAssets.LoadAssetSync<TextAsset>("Assets/GameRes/Raws/Scripts/" + resName);
        var ta = handle.AssetObject as TextAsset;
        handle.Release();

        return ta;
    }
}