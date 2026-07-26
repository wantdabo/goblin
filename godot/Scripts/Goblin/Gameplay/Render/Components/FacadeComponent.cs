using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Kowtow.Math;

namespace Goblin.Gameplay.Render.Components;

/// <summary>
/// 外观组件 — FacadeInfo 的纯数据投影
/// </summary>
[ProjectorTarget(typeof(FacadeInfo))]
public sealed partial class FacadeComponent : Component, IGBL
{
    /// <summary>
    /// 模型资源 ID
    /// </summary>
    public int model { get; set; }
    /// <summary>
    /// 动画推进模式
    /// </summary>
    public byte animticktype { get; set; }
    /// <summary>
    /// 动画状态
    /// </summary>
    public byte animstate { get; set; }
    /// <summary>
    /// 动画资源哈希
    /// </summary>
    public uint animhash { get; set; }
    /// <summary>
    /// 动画流逝时间
    /// </summary>
    public FP animelapsed { get; set; }
    /// <summary>
    /// 特效版本号
    /// </summary>
    public uint effectincrement { get; set; }
    /// <summary>
    /// 待移除特效列表
    /// </summary>
    public List<uint> rmveffects { get; set; }
    /// <summary>
    /// 特效字典
    /// </summary>
    public Dictionary<uint, EffectInfo> effectdict { get; set; }
    /// <summary>
    /// 动画槽位列表
    /// </summary>
    public List<AnimationSlot> animslots { get; set; }
}
