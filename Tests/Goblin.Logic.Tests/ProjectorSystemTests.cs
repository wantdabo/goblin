using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Behaviors.Sa;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Projection.Core;
using Goblin.Logic.Standalone.TestFixtures;
using Kowtow.Math;

namespace Goblin.Logic.Tests;

/// <summary>
/// T1.6 ProjectorSystem 测试
/// </summary>
[Collection("GBL")]
public class ProjectorSystemTests
{
    // ============================================================
    // ProjectorPacket
    // ============================================================

    [Fact]
    public void ProjectorPacket_DefaultValues()
    {
        var packet = new ProjectorPacket();

        Assert.Equal(0ul, packet.actor);
        Assert.Equal(0ul, packet.fieldmask);
        Assert.Equal(0L, packet.frame);
        Assert.Equal(0, packet.latency);
        Assert.Null(packet.values);
        Assert.Null(packet.addedkeys);
        Assert.Null(packet.removedkeys);
    }

    [Fact]
    public void ProjectorPacket_SetProperties()
    {
        var values = new object[] { 100, 200 };
        var packet = new ProjectorPacket
        {
            actor = 42,
            behaviorinfotype = typeof(TestProjectInfo),
            fieldmask = 3,
            frame = 10,
            values = values,
        };

        Assert.Equal(42ul, packet.actor);
        Assert.Equal(typeof(TestProjectInfo), packet.behaviorinfotype);
        Assert.Equal(3ul, packet.fieldmask);
        Assert.Equal(10L, packet.frame);
        Assert.Same(values, packet.values);
    }

    // ============================================================
    // IProjectable
    // ============================================================

    /// <summary>
    /// 非 [Projector] 类不实现 IProjectable
    /// </summary>
    [Fact]
    public void NonProjectorInfo_NotIProjectable()
    {
        var info = new TestProjectInfo();
        info.Ready(1);

        Assert.False(info is IProjectable);
    }

    // ============================================================
    // ProjectorSystem 自检遍历
    // ============================================================

    /// <summary>
    /// 新建 BehaviorInfo 首帧全量同步（MarkAllDirty 置全量 mask）
    /// </summary>
    [Fact]
    public void OnEndTick_NewInfo_FullSync()
    {
        var stage = new Stage();
        var projector = new ProjectorSystem();
        projector.Assemble(stage, stage.sa);

        stage.AddBehaviorInfo<ProjectFieldInfo>(1);

        // 诊断：直接验证 behaviorinfodict 迭代
        ulong foundActor = ulong.MaxValue;
        foreach (var (actorId, dict) in stage.cache.behaviorinfodict)
        {
            foundActor = actorId;
            Assert.True(dict.TryGetValue(typeof(ProjectFieldInfo), out var info));
            Assert.True(info is IProjectable);
            var proj = (IProjectable)info;
            Assert.True(0 != proj.projectdirtymask, $"mask={proj.projectdirtymask}");
        }
        Assert.Equal(1ul, foundActor);

        projector.EndTick();

        Assert.Single(projector.packets);
        var pkt = projector.packets[0];
        Assert.Equal(1ul, pkt.actor);
        Assert.Equal(typeof(ProjectFieldInfo), pkt.behaviorinfotype);
        // position(index 0) | scale(index 1)
        Assert.Equal(3ul, pkt.fieldmask);
        Assert.Equal(2, pkt.values.Length);
    }

    /// <summary>
    /// 无脏数据时 packets 为空
    /// </summary>
    [Fact]
    public void OnEndTick_NoDirty_NoPackets()
    {
        var stage = new Stage();
        var projector = new ProjectorSystem();
        projector.Assemble(stage, stage.sa);

        stage.AddBehaviorInfo<ProjectFieldInfo>(1);
        projector.EndTick();

        // 第二帧无脏
        projector.EndTick();

        Assert.Empty(projector.packets);
    }

    /// <summary>
    /// 部分字段变更只投影对应位
    /// </summary>
    [Fact]
    public void OnEndTick_PartialDirty_PartialSync()
    {
        var stage = new Stage();
        var projector = new ProjectorSystem();
        projector.Assemble(stage, stage.sa);

        var info = stage.AddBehaviorInfo<ProjectFieldInfo>(1);
        projector.EndTick();

        // 只改 position（index 0）
        info.position = new FPVector3(1, 2, 3);
        projector.EndTick();

        Assert.Single(projector.packets);
        Assert.Equal(1ul, projector.packets[0].fieldmask);
        Assert.Single(projector.packets[0].values);
    }

    /// <summary>
    /// 非 IProjectable 的 BehaviorInfo 被跳过
    /// </summary>
    [Fact]
    public void OnEndTick_NonProjectable_Skipped()
    {
        var stage = new Stage();
        var projector = new ProjectorSystem();
        projector.Assemble(stage, stage.sa);

        stage.AddBehaviorInfo<TestProjectInfo>(1);
        projector.EndTick();

        Assert.Empty(projector.packets);
    }

    /// <summary>
    /// 每帧 OnEndTick 开头回收上帧投影包
    /// </summary>
    [Fact]
    public void OnEndTick_RecyclesPrevPackets()
    {
        var stage = new Stage();
        var projector = new ProjectorSystem();
        projector.Assemble(stage, stage.sa);

        stage.AddBehaviorInfo<ProjectFieldInfo>(1);
        projector.EndTick();
        var firstCount = projector.packets.Length;

        // 第二帧无脏，回收上帧包
        projector.EndTick();

        Assert.Equal(1, firstCount);
        Assert.Empty(projector.packets);
    }

    /// <summary>
    /// 多 Actor 各自投影
    /// </summary>
    [Fact]
    public void OnEndTick_MultiActor_SeparatePackets()
    {
        var stage = new Stage();
        var projector = new ProjectorSystem();
        projector.Assemble(stage, stage.sa);

        stage.AddBehaviorInfo<ProjectFieldInfo>(1);
        stage.AddBehaviorInfo<ProjectFieldInfo>(2);
        projector.EndTick();

        Assert.Equal(2, projector.packets.Length);
        Assert.Equal(1ul, projector.packets[0].actor);
        Assert.Equal(2ul, projector.packets[1].actor);
    }
}

/// <summary>
/// 测试用 BehaviorInfo（无 [Projector]，非 IProjectable）
/// </summary>
public class TestProjectInfo : BehaviorInfo
{
    public int value { get; set; }

    protected override void OnReady()
    {
    }
}
