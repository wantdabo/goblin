//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using HybridCLR;
//using UnityEngine;

///// <summary>
///// 热更新逻辑加载模块
///// </summary>
//public class HotfixScript
//{
//    private static MethodInfo tickFunc;
//    private static MethodInfo fixedTickFunc;
    
//    public static void Init()
//    {
//#if !UNITY_EDITOR
//        List<string> aotMetaAssemblyFiles = new List<string>()
//        {
//            "mscorlib.dll.bytes",
//            "System.dll.bytes",
//            "System.Core.dll.bytes",
//            "LubanLib.dll.bytes",
//            "UniTask.dll.bytes",
//            "Animancer.dll.bytes",
//        };
//        HomologousImageMode mode = HomologousImageMode.SuperSet;
//        foreach (var aotDllName in aotMetaAssemblyFiles)
//        {
//            byte[] dllBytes = File.ReadAllBytes($"{Application.streamingAssetsPath}/Scripts/{aotDllName}");
//            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
//        }
//        Assembly ass = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/Scripts/Goblin.dll.bytes"));
//#else
//        Assembly ass = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Goblin");
//#endif
//        var export = ass.GetType("GoblinFramework.Core.HotfixExport");
//        export.GetMethod("InitHotfix").Invoke(null, null);
//        tickFunc = export.GetMethod("Tick");
//        fixedTickFunc = export.GetMethod("FixedTick");
//    }

//    public static void Tick()
//    {
//        tickFunc.Invoke(null, new object[] { Time.deltaTime });
//    }

//    public static void FixedTick()
//    {
//        fixedTickFunc.Invoke(null, new object[] { Time.fixedDeltaTime });
//    }
//}