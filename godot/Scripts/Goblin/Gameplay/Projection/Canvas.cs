using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Projection.Core;
using Goblin.Gameplay.Projection.Shadows;
using Kowtow.Math;

namespace Goblin.Gameplay.Projection;

/// <summary>
/// 数据画布 — Logic 层 BehaviorInfo 数据的纯数据主线程副本
/// ActorID → (ShadowType → Shadow实例) 扁平组织
/// </summary>
public partial class Canvas
{
    /// <summary>
    /// InfoType → Shadow 注册条目（factory + apply + shadowtype 合一，单次查找拿到全部）
    /// </summary>
    private struct ShadowEntry
    {
        /// <summary>
        /// Shadow 类型（shadowdict 的 key）
        /// </summary>
        public Type shadowtype;
        /// <summary>
        /// 工厂委托（零反射创建 Shadow 实例）
        /// </summary>
        public Func<Shadow> factory;
        /// <summary>
        /// ApplyTo 静态委托（包装后入参为 object）
        /// </summary>
        public Action<object, ulong, object[]> apply;
    }

    /// <summary>
    /// ActorID → (ShadowType → Shadow实例)
    /// </summary>
    private Dictionary<ulong, Dictionary<Type, Shadow>> datas { get; set; }
    /// <summary>
    /// BehaviorInfo 类型 → Shadow 注册条目（factory + apply + shadowtype）
    /// 合并原 infotoshadow/applymap/factorymap 三表，Apply 时字典查找 5→3 次
    /// </summary>
    private Dictionary<Type, ShadowEntry> infomap { get; set; }
    /// <summary>
    /// 未注册 infotype 去重缓存（避免重复 Warning）
    /// </summary>
    private HashSet<Type> missinginfologged { get; set; }

    public Canvas()
    {
        datas = new Dictionary<ulong, Dictionary<Type, Shadow>>();
        infomap = new Dictionary<Type, ShadowEntry>();
        missinginfologged = new HashSet<Type>();
    }

    /// <summary>
    /// 注册 BehaviorInfo → Shadow 映射
    /// ApplyTo 委托包装类型转换，工厂委托零反射创建
    /// </summary>
    public void Register<TInfo, TShadow>()
        where TShadow : Shadow, IShadowApply<TShadow>, new()
    {
        infomap[typeof(TInfo)] = new ShadowEntry
        {
            shadowtype = typeof(TShadow),
            factory = () => ObjectPool.Ensure<TShadow>(),
            apply = (obj, mask, vals) => TShadow.ApplyTo((TShadow)obj, mask, vals)
        };
    }

    /// <summary>
    /// 获取指定 Actor 的影子
    /// </summary>
    public T GetShadow<T>(ulong actor) where T : Shadow
    {
        if (datas.TryGetValue(actor, out var shadowdict)
            && shadowdict.TryGetValue(typeof(T), out var shadow))
            return shadow as T;
        return null;
    }

    /// <summary>
    /// 查询 Actor 的 SpatialShadow 位置（用于 AOI 裁剪）
    /// </summary>
    public FPVector3? TryGetPosition(ulong actor)
    {
        var shadow = GetShadow<SpatialShadow>(actor);
        if (null == shadow) return null;
        return shadow.position;
    }

    /// <summary>
    /// 查询 Actor 是否在当前 Canvas 中有数据（用于可见性裁剪）
    /// </summary>
    public bool HasActor(ulong actor)
    {
        return datas.ContainsKey(actor);
    }

    /// <summary>
    /// 应用单条投影数据
    /// 单次 infomap 查找拿到 shadowtype + factory + apply，省去原 applymap/factorymap 两次查找
    /// </summary>
    private void Apply(ulong actor, Type infotype, ulong fieldmask, object[] values)
    {
        if (false == infomap.TryGetValue(infotype, out var entry))
        {
            // 未注册的 InfoType 打 Warning（去重）
            if (false == missinginfologged.Contains(infotype))
            {
                missinginfologged.Add(infotype);
                System.Diagnostics.Debug.WriteLine($"Canvas.Apply: BehaviorInfo 类型 '{infotype.FullName}' 未通过 Register<> 注册，投影数据被丢弃");
            }
            return;
        }

        if (false == datas.TryGetValue(actor, out var shadowdict))
        {
            shadowdict = new Dictionary<Type, Shadow>();
            datas[actor] = shadowdict;
        }

        if (false == shadowdict.TryGetValue(entry.shadowtype, out var shadow))
        {
            shadow = entry.factory();
            shadowdict[entry.shadowtype] = shadow;
        }

        entry.apply(shadow, fieldmask, values);
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
    /// 移除 Actor 的所有数据，Shadow 实例归还对象池
    /// </summary>
    public void RmvActor(ulong actor)
    {
        if (datas.TryGetValue(actor, out var shadowdict))
        {
            foreach (var val in shadowdict.Values)
            {
                if (val is IGBL gbl)
                {
                    gbl.Reset();
                    ObjectPool.Set(gbl);
                }
            }
            shadowdict.Clear();
            datas.Remove(actor);
        }
    }
}
