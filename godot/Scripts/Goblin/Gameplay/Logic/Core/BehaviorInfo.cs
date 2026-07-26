using Goblin.Common;

namespace Goblin.Gameplay.Logic.Core;

/// <summary>
/// 行为信息，类似 ECS 中的 Component
/// 实现 IGBL 接口，Source Generator 扫描 partial class + IGBL 自动生成 override SGReady / SGReset / SGClone
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
        SGReady();
        OnReady();
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        SGReset();
        OnReset();
        this.actor = 0;
        this.active = false;
    }

    /// <summary>
    /// 克隆
    /// </summary>
    public BehaviorInfo Clone()
    {
        var c = SGClone();
        c.OnClone();
        return c;
    }

    /// <summary>
    /// IGBL 接口 Clone
    /// </summary>
    IGBL IGBL.Clone()
    {
        return Clone();
    }

    // ---- SG 覆盖点（SG 生成 override） ----

    /// <summary>
    /// SG 生成的 Ready 初始化，仅初始化容器字段
    /// </summary>
    protected virtual void SGReady() { }

    /// <summary>
    /// SG 生成的 Reset 清理，归零所有字段
    /// </summary>
    protected virtual void SGReset() { }

    /// <summary>
    /// SG 生成的 Clone 创建与字段拷贝，基类返回 this
    /// </summary>
    protected virtual BehaviorInfo SGClone()
    {
        return this;
    }

    // ---- 用户钩子 ----

    /// <summary>
    /// 用户自定义 Ready 初始化，子类可 override 初始化值类型字段
    /// </summary>
    protected virtual void OnReady() { }

    /// <summary>
    /// 用户自定义 Reset，子类可 override 重置自定义数据
    /// </summary>
    protected virtual void OnReset() { }

    /// <summary>
    /// Clone 后回调，子类可 override 做克隆后处理
    /// </summary>
    protected virtual void OnClone() { }
}
