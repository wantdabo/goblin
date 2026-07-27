using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 特效信息
/// </summary>
public struct EffectInfo
{
    /// <summary>
    /// 特效 ID
    /// </summary>
    public uint id { get; set; }
    /// <summary>
    /// 流逝时间
    /// </summary>
    public FP elapsed { get; set; }
    /// <summary>
    /// 特效资源 ID
    /// </summary>
    public int effect { get; set; }
    /// <summary>
    /// 特效类型
    /// </summary>
    public byte type { get; set; }
    /// <summary>
    /// 特效跟随
    /// </summary>
    public byte follow { get; set; }
    /// <summary>
    /// 挂点
    /// </summary>
    public ushort mount { get; set; }
    /// <summary>
    /// 特效跟随掩码
    /// </summary>
    public int followmask { get; set; }
    /// <summary>
    /// 特效持续时间
    /// </summary>
    public FP duration { get; set; }
    /// <summary>
    /// 特效位置
    /// </summary>
    public FPVector3 position { get; set; }
    /// <summary>
    /// 特效旋转
    /// </summary>
    public FPVector3 euler { get; set; }
    /// <summary>
    /// 特效缩放
    /// </summary>
    public FP scale { get; set; }
}

/// <summary>
/// 动画槽位
/// </summary>
public partial class AnimationSlot : IGBL
{
    /// <summary>
    /// 复合槽位键（高字节=槽位类型，低字节=动画层）
    /// </summary>
    public ushort key { get; set; }
    /// <summary>
    /// 优先级（越大越优先）
    /// </summary>
    public int priority { get; set; }
    /// <summary>
    /// 持久状态
    /// </summary>
    public byte animstate { get; set; }
    /// <summary>
    /// 动画名称哈希
    /// </summary>
    public uint animhash { get; set; }
    /// <summary>
    /// 动画层
    /// </summary>
    public byte layer { get; set; }
    /// <summary>
    /// 是否活跃
    /// </summary>
    public bool active { get; set; }
    /// <summary>
    /// 临时覆盖
    /// </summary>
    public bool istransient { get; set; }
    /// <summary>
    /// 剩余时间
    /// </summary>
    public FP duration { get; set; }
    /// <summary>
    /// 槽位独立流逝时间
    /// </summary>
    public FP elapsed { get; set; }
}

/// <summary>
/// 外观信息
/// </summary>
// 模型资源 ID
[Projector("model", typeof(int))]
// 动画推进模式
[Projector("animticktype", typeof(byte))]
// 动画状态
[Projector("animstate", typeof(byte))]
// 动画资源哈希
[Projector("animhash", typeof(uint))]
// 动画流逝时间
[Projector("animelapsed", typeof(FP))]
// 特效版本号
[Projector("effectincrement", typeof(uint))]
// 待移除特效列表
[Projector("rmveffects", typeof(TGBLList<uint>))]
// 特效字典
[Projector("effectdict", typeof(TGBLDict<uint, EffectInfo>))]
// 动画槽位列表
[Projector("animslots", typeof(TGBLList<AnimationSlot>))]
public partial class FacadeInfo : BehaviorInfo
{
}