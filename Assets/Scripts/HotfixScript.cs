using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YooAsset;

/// <summary>
/// 热更新逻辑加载模块
/// </summary>
public class HotfixScript
{
    private static MethodInfo tickFunc;
    private static MethodInfo fixedTickFunc;

    private static string scriptsPath = "Assets/GameRawRes/Scripts/";

    private static void LoadMetadata()
    {
        var handle = YooAssets.LoadRawFileSync($"{scriptsPath}AOT_DLL_LIST");
        string[] aotDllList = handle.GetRawFileText().Split('|');
        handle.Release();

        foreach (var aotDllName in aotDllList)
        {
            handle = YooAssets.LoadRawFileSync($"{scriptsPath}{aotDllName}");
            var code = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(handle.GetRawFileData(), HybridCLR.HomologousImageMode.SuperSet);
            handle.Release();
            Debug.Log($"LoadMetadata: {aotDllName}. ret: {code}");
        }
    }

    public static void Init()
    {
        LoadMetadata();
#if !UNITY_EDITOR
        var handle = YooAssets.LoadRawFileSync($"{scriptsPath}Goblin.dll");
        Assembly ass = Assembly.Load(handle.GetRawFileData());
        handle.Release();
#else
        Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Goblin");
#endif
        var export = ass.GetType("Goblin.Core.GoblinExport");
        export.GetMethod("Init").Invoke(null, null);
        tickFunc = export.GetMethod("Tick");
        fixedTickFunc = export.GetMethod("FixedTick");
    }

    public static void Tick()
    {
        tickFunc?.Invoke(null, new object[] { Time.deltaTime });
    }

    public static void FixedTick()
    {
        fixedTickFunc?.Invoke(null, new object[] { Time.fixedDeltaTime });
    }
}