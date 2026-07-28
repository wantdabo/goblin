using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors.Sa;

/// <summary>
/// 事件标记接口
/// </summary>
public interface IEvent { }

/// <summary>
/// 事件订阅派发者（静态单例）
/// 订阅方在静态 ctor 中调用 Listen 注册
/// 派发方调用 Tell 通知所有订阅方
/// 按 handler 所在类型全名（Ordinal）确定时序，跨平台一致
/// </summary>
public static class Eventor
{
    /// <summary>
    /// 按类型名排序用的比较器
    /// </summary>
    private static readonly EntryComparer comparer = new();

    /// <summary>
    /// 事件字典 [事件类型 → 处理器列表]
    /// </summary>
    private static readonly Dictionary<Type, List<(string key, Delegate action)>> eventdict = new();

    /// <summary>
    /// 注册事件监听
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="handler">静态处理函数</param>
    public static void Listen<T>(Action<Stage, T> handler) where T : IEvent
    {
        var type = typeof(T);
        if (false == eventdict.TryGetValue(type, out var list))
        {
            list = new List<(string, Delegate)>();
            eventdict.Add(type, list);
        }

        string key = handler.Method.DeclaringType.FullName;
        list.Add((key, handler));
        list.Sort(comparer);
    }

    /// <summary>
    /// 派发事件
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="stage">逻辑阶段</param>
    /// <param name="e">事件参数</param>
    public static void Tell<T>(Stage stage, T e) where T : IEvent
    {
        if (null == stage) return;
        if (false == eventdict.TryGetValue(typeof(T), out var list)) return;
        for (int i = 0; i < list.Count; i++)
        {
            var entry = list[i];
            (entry.action as Action<Stage, T>).Invoke(stage, e);
        }
    }

    /// <summary>
    /// 条目排序比较器：按类型全名字母序
    /// </summary>
    private sealed class EntryComparer : IComparer<(string key, Delegate action)>
    {
        public int Compare((string key, Delegate action) x, (string key, Delegate action) y)
        {
            return string.CompareOrdinal(x.key, y.key);
        }
    }
}
