using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Kowtow.Math;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// 碰撞指令数据
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class CollisionData : InstructData
{
    public override ushort id => INSTR_DEFINE.COLLISION;

    /// <summary>
    /// 类型
    /// </summary>
    public byte type = COLLISION_DEFINE.COLLISION_TYPE_HURT;
        
    /// <summary>
    /// 检测类型
    /// </summary>
    public byte overlaptype = COLLISION_DEFINE.COLLISION_BOX;
    /// <summary>
    /// 包括执行目标
    /// </summary>
    public bool includetarget = false;
    /// <summary>
    /// 包括死亡单位
    /// </summary>
    public bool includedead = false;
    /// <summary>
    /// 最大检测次数
    /// </summary>
    public uint count = 1;
    /// <summary>
    /// 偏移
    /// </summary>
    public IntVector3 offset;
    /// <summary>
    /// 射线方向
    /// </summary>
    public IntVector3 raydire = new(0, 0, 1000);
    /// <summary>
    /// 射线长度
    /// </summary>
    public uint raydis = 1000;
    /// <summary>
    /// 线段终点
    /// </summary>
    public IntVector3 lineep = new(0, 0, 1000);
    /// <summary>
    /// 立方体大小
    /// </summary>
    public IntVector3 boxsize = new(1000, 1000, 1000);
    /// <summary>
    /// 球体半径
    /// </summary>
    public uint sphereradius = 500;
    /// <summary>
    /// 使用[自身]命中火花
    /// </summary>
    public bool usespark;
    /// <summary>
    /// [自身]命中火花
    /// </summary>
    public SparkData spark;
}