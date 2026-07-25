using System.Collections.Generic;
using Goblin.Gameplay.Projection.Core;

namespace Goblin.Gameplay.Projection.Rules;

/// <summary>
/// 权限裁剪规则 — 按 (ObserverType, BehaviorInfoType) 查表，返回允许的 fieldmask
/// 敌方只能同步位置，队友同步全部字段
/// </summary>
public class PermissionRule : IProjectionRule
{
    /// <summary>
    /// 权限表：键为 (ObserverType, BehaviorInfoType)，值为允许的 fieldmask 位图
    /// </summary>
    private Dictionary<(ObserverType, System.Type), ulong> permtable { get; set; }

    public PermissionRule()
    {
        permtable = new Dictionary<(ObserverType, System.Type), ulong>();
    }

    /// <summary>
    /// 注册权限规则
    /// </summary>
    /// <param name="observerType">观察者类型</param>
    /// <param name="behaviorInfoType">BehaviorInfo 类型</param>
    /// <param name="allowedMask">允许的字段掩码（0 表示完全禁止）</param>
    public void Add(ObserverType observerType, System.Type behaviorInfoType, ulong allowedMask)
    {
        permtable[(observerType, behaviorInfoType)] = allowedMask;
    }

    /// <summary>
    /// 裁剪：按权限表 AND 当前 mask
    /// </summary>
    public ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask)
    {
        if (0 == currentmask) return 0;

        var key = (observer.type, packet.behaviorinfotype);
        if (false == permtable.TryGetValue(key, out var allowed)) return currentmask;

        return currentmask & allowed;
    }
}
