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
public partial class FacadeInfo : BehaviorInfo
{
    /// <summary>
    /// 模型 ID
    /// </summary>
    public int model { get; set; }
    /// <summary>
    /// 动画更新类型
    /// </summary>
    public byte animticktype { get; set; }
    /// <summary>
    /// 动画状态
    /// </summary>
    public byte animstate { get; set; }
    /// <summary>
    /// 动画名称哈希
    /// </summary>
    public uint animhash { get; set; }
    /// <summary>
    /// 流逝时间
    /// </summary>
    public FP animelapsed { get; set; }
    /// <summary>
    /// 特效增量 ID
    /// </summary>
    public uint effectincrement { get; set; }
    /// <summary>
    /// 移除特效列表
    /// </summary>
    public GBLList<uint> rmveffects { get; set; }
    /// <summary>
    /// 特效列表
    /// </summary>
    public GBLList<uint> effects { get; set; }
    /// <summary>
    /// 特效字典
    /// </summary>
    public GBLDict<uint, EffectInfo> effectdict { get; set; }
    /// <summary>
    /// 动画槽位列表
    /// </summary>
    public GBLList<AnimationSlot> animslots { get; set; }
}