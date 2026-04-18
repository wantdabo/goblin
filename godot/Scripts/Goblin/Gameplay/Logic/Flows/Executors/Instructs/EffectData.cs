using System;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Kowtow.Math;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// 特效指令数据
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class EffectData : InstructData
{
    public override ushort id => INSTR_DEFINE.EFFECT;

    /// <summary>
    /// 特效资源 ID
    /// </summary>
    public int effect;

    /// <summary>
    /// 特效类型
    /// </summary>
    public byte type;

    /// <summary>
    /// 是否随管线回收
    /// </summary>
    public bool recywithflow;

    /// <summary>
    /// 特效持续时间类型
    /// </summary>
    public byte durationtype = EFFECT_DEFINE.DURATION_TIMELINE;

    /// <summary>
    /// 特效持续时间(毫秒)
    /// </summary>
    public int duration = 1000;

    /// <summary>
    /// 特效跟随
    /// </summary>
    public byte follow;

    /// <summary>
    /// 挂点
    /// </summary>
    public ushort mount;

    /// <summary>
    /// 特效跟随掩码
    /// </summary>
    public int followmask;

    /// <summary>
    /// 特效位置
    /// </summary>
    public IntVector3 position;

    /// <summary>
    /// 特效旋转
    /// </summary>
    public IntVector3 euler;

    /// <summary>
    /// 特效缩放
    /// </summary>
    public int scale = 1000;
}