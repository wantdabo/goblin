using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HybridCLR;

/// <summary>
/// 热更新逻辑加载模块
/// </summary>
public class HotfixScript
{
    private static MethodInfo tickFunc;
    private static MethodInfo fixedTickFunc;

    public static void Init()
    {
#if !UNITY_EDITOR
        Assembly ass = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/Goblin.dll.bytes"));
#else
        Assembly ass = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Goblin");
#endif
        var export = ass.GetType("Goblin.Core.GoblinExport");
        export.GetMethod("InitGoblin").Invoke(null, null);
        tickFunc = export.GetMethod("Tick");
        fixedTickFunc = export.GetMethod("FixedTick");
    }

    public static void Tick()
    {
        tickFunc.Invoke(null, new object[] { Time.deltaTime });
    }

    public static void FixedTick()
    {
        fixedTickFunc.Invoke(null, new object[] { Time.fixedDeltaTime });
    }
}