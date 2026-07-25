using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;

namespace Goblin.Gameplay.Projection;

/// <summary>
/// 表现世界 — 管理所有 Entity，接收 Transport 推送的投影数据
/// BehaviorInfo 类型 → Component 类型映射决定哪个 Component 接收数据
/// </summary>
public class RenderWorld
{
    /// <summary>
    /// 实体字典，键为 ActorID
    /// </summary>
    private Dictionary<ulong, Entity> entities { get; set; }
    /// <summary>
    /// BehaviorInfo 类型 → Component 类型 映射
    /// </summary>
    private Dictionary<Type, Type> behaviortocomp { get; set; }

    /// <summary>
    /// 实体创建事件
    /// </summary>
    public event Action<Entity> OnEntityCreated;
    /// <summary>
    /// 实体移除事件
    /// </summary>
    public event Action<Entity> OnEntityRemoved;

    public RenderWorld()
    {
        entities = ObjectPool.Ensure<Dictionary<ulong, Entity>>();
        behaviortocomp = ObjectPool.Ensure<Dictionary<Type, Type>>();
    }

    /// <summary>
    /// 注册 BehaviorInfo → Component 映射
    /// </summary>
    /// <typeparam name="TBehaviorInfo">BehaviorInfo 类型</typeparam>
    /// <typeparam name="TComponent">Component 类型</typeparam>
    public void RegisterMapping<TBehaviorInfo, TComponent>() where TComponent : Component, new()
    {
        behaviortocomp[typeof(TBehaviorInfo)] = typeof(TComponent);
    }

    /// <summary>
    /// 应用投影数据到 Entity.Component
    /// </summary>
    /// <param name="actor">ActorID</param>
    /// <param name="behaviorinfotype">BehaviorInfo 类型</param>
    /// <param name="fieldmask">脏字段掩码</param>
    /// <param name="values">脏字段值数组</param>
    public void Apply(ulong actor, Type behaviorinfotype, ulong fieldmask, object[] values)
    {
        // 确保 Entity 存在
        if (false == entities.TryGetValue(actor, out var entity))
        {
            entity = new Entity(actor);
            entities[actor] = entity;
            OnEntityCreated?.Invoke(entity);
        }

        // 无映射的 BehaviorInfo 不创建 Component
        if (false == behaviortocomp.TryGetValue(behaviorinfotype, out var comptype)) return;

        // 确保 Component 存在
        var comp = entity.GetComp(comptype);
        if (null == comp)
        {
            comp = (Component)ObjectPool.Ensure(comptype);
            entity.AddComp(comp);
        }

        // 直接写入数据
        comp.Apply(fieldmask, values);
    }

    /// <summary>
    /// 批量应用 ObserverPacket（Transport 调用入口）
    /// </summary>
    /// <param name="packets">裁剪后的观察者数据包</param>
    public void ApplyPackets(ObserverPacket[] packets)
    {
        if (null == packets) return;
        foreach (var p in packets)
        {
            Apply(p.actor, p.behaviorinfotype, p.fieldmask, p.values);
        }
    }

    /// <summary>
    /// 移除实体
    /// </summary>
    /// <param name="actor">ActorID</param>
    public void RmvEntity(ulong actor)
    {
        if (entities.TryGetValue(actor, out var entity))
        {
            entity.Destroy();
            entities.Remove(actor);
            OnEntityRemoved?.Invoke(entity);
        }
    }

    /// <summary>
    /// 获取实体
    /// </summary>
    /// <param name="actor">ActorID</param>
    /// <returns>实体，不存在返回 null</returns>
    public Entity GetEntity(ulong actor)
    {
        return entities.TryGetValue(actor, out var e) ? e : null;
    }
}
