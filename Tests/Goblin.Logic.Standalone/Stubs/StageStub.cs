using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Projection;

namespace Goblin.Gameplay.Logic.Core;

/// <summary>
/// Stage 最小化桩类型，仅用于 Standalone 项目编译
/// 提供 Behavior / Behavior&lt;T&gt; 所需的方法签名
/// </summary>
public sealed class Stage
{
    /// <summary>
    /// System Actor ID
    /// </summary>
    public ulong sa => ulong.MaxValue;

    /// <summary>
    /// 当前帧号
    /// </summary>
    public uint frame => 0;

    /// <summary>
    /// Stage 缓存（桩实现，供 ProjectorSystem 自检遍历 behaviorinfodict）
    /// </summary>
    public StageCache cache { get; set; } = new StageCache();

    /// <summary>
    /// 获取 BehaviorInfo（桩实现）
    /// </summary>
    public T GetBehaviorInfo<T>(ulong id, bool force) where T : BehaviorInfo
    {
        return default;
    }

    /// <summary>
    /// 查找 BehaviorInfo（桩实现）
    /// </summary>
    public bool SeekBehaviorInfo<T>(ulong id, out T info, bool force = false) where T : BehaviorInfo
    {
        info = default;
        return false;
    }

    /// <summary>
    /// 添加 BehaviorInfo（加入 behaviorinfodict，供 ProjectorSystem 自检遍历）
    /// </summary>
    public T AddBehaviorInfo<T>(ulong id) where T : BehaviorInfo, new()
    {
        if (false == cache.behaviorinfodict.TryGetValue(id, out var dict))
        {
            dict = new Dictionary<Type, BehaviorInfo>();
            cache.behaviorinfodict[id] = dict;
        }

        var info = ObjectCache.Ensure<T>();
        dict[typeof(T)] = info;
        info.Ready(id);

        // 新对象首帧全量同步
        if (info is IProjectable proj) proj.MarkAllDirty();

        return info;
    }

    /// <summary>
    /// 移除 BehaviorInfo（桩实现）
    /// </summary>
    public void RmvBehaviorInfo(BehaviorInfo info)
    {
    }
}

/// <summary>
/// Stage 缓存桩类型，仅提供 ProjectorSystem 自检所需的 behaviorinfodict
/// </summary>
public sealed class StageCache
{
    /// <summary>
    /// 行为信息列表，键为 ActorID，值为该 Actor 上的所有行为信息
    /// </summary>
    public Dictionary<ulong, Dictionary<Type, BehaviorInfo>> behaviorinfodict { get; set; } = new Dictionary<ulong, Dictionary<Type, BehaviorInfo>>();
}
