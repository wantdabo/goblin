using Goblin.Common;

namespace Goblin.Gameplay.Logic.Core;

/// <summary>
/// 行为信息，类似 ECS 中的 Component
/// 实现 IGBL 接口，Source Generator 扫描 partial class + IGBL 自动生成 override Reset / Clone
/// </summary>
public abstract class BehaviorInfo : IGBL
{
    /// <summary>
    /// ActorID
    /// </summary>
    public ulong actor { get; private set; }
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool active { get; set; }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="actor">ActorID</param>
    public void Ready(ulong actor)
    {
        this.actor = actor;
        this.active = true;
        OnInitContainers();
        OnReady();
    }

    /// <summary>
    /// 容器字段初始化（SG 为含 GBL 容器的 BehaviorInfo 子类生成 override）
    /// </summary>
    protected virtual void OnInitContainers() { }

    /// <summary>
    /// 重置，virtual — SG 为 partial class + IGBL 类生成 override
    /// override 中清理字段后尾调 base.Reset()
    /// </summary>
    public virtual void Reset()
    {
        OnReset();
        this.actor = 0;
        this.active = false;
    }

    /// <summary>
    /// 克隆，返回 BehaviorInfo（遗留兼容）
    /// virtual — SG 为 partial class + IGBL 类生成 override
    /// </summary>
    public virtual BehaviorInfo Clone()
    {
        return OnClone();
    }

    /// <summary>
    /// IGBL 接口 Clone，T1.11 替换遗留 Clone 后统一使用
    /// </summary>
    IGBL IGBL.Clone()
    {
        return Clone();
    }

    /// <summary>
    /// 初始化，当 BehaviorInfo 从对象池中取出，在这个回调中初始化数据
    /// </summary>
    protected virtual void OnReady() { }
    /// <summary>
    /// 重置，当 BehaviorInfo 回收，重新回到对象池，在这个回调中清理数据
    /// virtual 空实现 — 已有子类继续 override，新 partial 类由 SG 接管 Reset
    /// </summary>
    protected virtual void OnReset()
    {
    }
    /// <summary>
    /// 克隆，克隆一个新的 BehaviorInfo
    /// virtual 空实现 — 已有子类继续 override，新 partial 类由 SG 接管 Clone
    /// </summary>
    protected virtual BehaviorInfo OnClone()
    {
        return this;
    }
}
