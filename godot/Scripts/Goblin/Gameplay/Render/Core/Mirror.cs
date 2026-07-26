using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Projection.Core;
using Goblin.Gameplay.Render.Components;
using Kowtow.Math;

namespace Goblin.Gameplay.Render.Core;

/// <summary>
/// 数据镜像 — Logic 层 BehaviorInfo 数据的纯数据主线程副本
/// ActorID → (ComponentType → Component实例) 扁平组织
/// </summary>
public partial class Mirror
{
    /// <summary>
    /// ActorID → (ComponentType → Component实例)
    /// </summary>
    private Dictionary<ulong, Dictionary<Type, Component>> datas { get; set; }
    /// <summary>
    /// BehaviorInfo 类型 → Component 类型 映射
    /// </summary>
    private Dictionary<Type, Type> infotocomp { get; set; }
    /// <summary>
    /// Component 类型 → ApplyTo 静态委托（包装后入参为 object）
    /// </summary>
    private Dictionary<Type, Action<object, ulong, object[]>> applymap { get; set; }
    /// <summary>
    /// Component 类型 → 工厂委托（零反射创建）
    /// </summary>
    private Dictionary<Type, Func<Component>> factorymap { get; set; }
    /// <summary>
    /// 未注册 infotype 去重缓存（避免重复 Warning）
    /// </summary>
    private HashSet<Type> missinginfologged { get; set; }

    public Mirror()
    {
        datas = new Dictionary<ulong, Dictionary<Type, Component>>();
        infotocomp = new Dictionary<Type, Type>();
        applymap = new Dictionary<Type, Action<object, ulong, object[]>>();
        factorymap = new Dictionary<Type, Func<Component>>();
        missinginfologged = new HashSet<Type>();
    }

    /// <summary>
    /// 注册 BehaviorInfo → Component 映射
    /// ApplyTo 委托包装类型转换，工厂委托零反射创建
    /// </summary>
    public void Register<TInfo, TComp>()
        where TComp : Component, IComponentApply<TComp>, new()
    {
        infotocomp[typeof(TInfo)] = typeof(TComp);
        applymap[typeof(TComp)] = (obj, mask, vals) => TComp.ApplyTo((TComp)obj, mask, vals);
        factorymap[typeof(TComp)] = () => new TComp();
    }

    /// <summary>
    /// 获取指定 Actor 的组件
    /// </summary>
    public T GetComp<T>(ulong actor) where T : Component
    {
        if (datas.TryGetValue(actor, out var compdict)
            && compdict.TryGetValue(typeof(T), out var comp))
            return comp as T;
        return null;
    }

    /// <summary>
    /// 查询 Actor 的 SpatialComponent 位置（用于 AOI 裁剪）
    /// </summary>
    public FPVector3? TryGetPosition(ulong actor)
    {
        var comp = GetComp<SpatialComponent>(actor);
        if (null == comp) return null;
        return comp.position;
    }

    /// <summary>
    /// 查询 Actor 是否在当前 Mirror 中有数据（用于可见性裁剪）
    /// </summary>
    public bool HasActor(ulong actor)
    {
        return datas.ContainsKey(actor);
    }

    /// <summary>
    /// 应用单条投影数据
    /// </summary>
    private void Apply(ulong actor, Type infotype, ulong fieldmask, object[] values)
    {
        if (false == infotocomp.TryGetValue(infotype, out var comptype))
        {
            // 未注册的 InfoType 打 Warning（去重）
            if (false == missinginfologged.Contains(infotype))
            {
                missinginfologged.Add(infotype);
                System.Diagnostics.Debug.WriteLine($"Mirror.Apply: BehaviorInfo 类型 '{infotype.FullName}' 未通过 Register<> 注册，投影数据被丢弃");
            }
            return;
        }

        if (false == datas.TryGetValue(actor, out var compdict))
        {
            compdict = new Dictionary<Type, Component>();
            datas[actor] = compdict;
        }

        if (false == compdict.TryGetValue(comptype, out var comp))
        {
            if (false == factorymap.TryGetValue(comptype, out var factory))
            {
                throw new InvalidOperationException(
                    $"Mirror.Apply: Component 类型 '{comptype.FullName}' 未通过 Register<> 注册。请确保启动时调用了 Mirror.Register<{comptype.Name}, TComp>()。");
            }
            comp = factory();
            compdict[comptype] = comp;
        }

        if (applymap.TryGetValue(comptype, out var apply))
        {
            apply(comp, fieldmask, values);
        }
    }

    /// <summary>
    /// 批量应用 ObserverPacket（每条 ObserverPacket 独立 Apply，按 Observer 合并）
    /// </summary>
    public void ApplyPackets(ObserverPacket[] packets)
    {
        if (null == packets) return;
        foreach (var p in packets)
        {
            Apply(p.actor, p.behaviorinfotype, p.fieldmask, p.values);
        }
    }

    /// <summary>
    /// 移除 Actor 的所有数据，Component 实例归还对象池
    /// </summary>
    public void RmvActor(ulong actor)
    {
        if (datas.TryGetValue(actor, out var compdict))
        {
            foreach (var val in compdict.Values)
            {
                if (val is IGBL gbl)
                {
                    gbl.Reset();
                    ObjectPool.Set(gbl);
                }
            }
            compdict.Clear();
            datas.Remove(actor);
        }
    }
}
