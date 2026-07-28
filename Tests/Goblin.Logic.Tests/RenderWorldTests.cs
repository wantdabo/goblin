using System;
using Goblin.Gameplay.Logic.Behaviors.Sa;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Projection;
using Goblin.Gameplay.Projection.Core;
using Goblin.Gameplay.Projection.Shadows;
using Goblin.Gameplay.Projection.Transport;
using Goblin.Logic.Standalone.TestFixtures;
using Kowtow.Math;

namespace Goblin.Logic.Tests;

/// <summary>
/// T1.9 Canvas / Shadow 测试 + 端到端投影链路
/// </summary>
[Collection("GBL")]
public class RenderWorldTests
{
    // ============================================================
    // Canvas / Shadow
    // ============================================================

    /// <summary>
    /// ApplyPackets 创建 Shadow，写入字段
    /// </summary>
    [Fact]
    public void Apply_CreatesShadow()
    {
        var canvas = new Canvas();
        canvas.Register<ProjectFieldInfo, TestSpatialShadow>();

        canvas.ApplyPackets(new ObserverPacket[]
        {
            new ObserverPacket
            {
                actor = 1,
                behaviorinfotype = typeof(ProjectFieldInfo),
                fieldmask = 3,
                values = new object[] { new FPVector3(1, 2, 3), new FP(5) }
            }
        });

        var shadow = canvas.GetShadow<TestSpatialShadow>(1);
        Assert.NotNull(shadow);
        Assert.Equal(new FPVector3(1, 2, 3), shadow.position);
        Assert.Equal(new FP(5), shadow.scale);
    }

    /// <summary>
    /// 无映射的 BehaviorInfo 不创建 Shadow
    /// </summary>
    [Fact]
    public void Apply_NoMapping_NoShadow()
    {
        var canvas = new Canvas();

        canvas.ApplyPackets(new ObserverPacket[]
        {
            new ObserverPacket
            {
                actor = 1,
                behaviorinfotype = typeof(ProjectFieldInfo),
                fieldmask = 3,
                values = new object[] { new FPVector3(1, 2, 3), FP.One }
            }
        });

        Assert.Null(canvas.GetShadow<TestSpatialShadow>(1));
    }

    /// <summary>
    /// RmvActor 移除 Actor 数据
    /// </summary>
    [Fact]
    public void RmvActor_RemovesData()
    {
        var canvas = new Canvas();
        canvas.Register<ProjectFieldInfo, TestSpatialShadow>();
        canvas.ApplyPackets(new ObserverPacket[]
        {
            new ObserverPacket
            {
                actor = 1,
                behaviorinfotype = typeof(ProjectFieldInfo),
                fieldmask = 3,
                values = new object[] { new FPVector3(1, 2, 3), FP.One }
            }
        });

        canvas.RmvActor(1);

        Assert.Null(canvas.GetShadow<TestSpatialShadow>(1));
    }

    /// <summary>
    /// 部分字段 Apply 只更新对应位
    /// </summary>
    [Fact]
    public void Apply_PartialMask_UpdatesPartial()
    {
        var canvas = new Canvas();
        canvas.Register<ProjectFieldInfo, TestSpatialShadow>();
        canvas.ApplyPackets(new ObserverPacket[]
        {
            new ObserverPacket
            {
                actor = 1,
                behaviorinfotype = typeof(ProjectFieldInfo),
                fieldmask = 3,
                values = new object[] { new FPVector3(1, 2, 3), new FP(5) }
            }
        });

        // 只更新 position（位 0）
        canvas.ApplyPackets(new ObserverPacket[]
        {
            new ObserverPacket
            {
                actor = 1,
                behaviorinfotype = typeof(ProjectFieldInfo),
                fieldmask = 1,
                values = new object[] { new FPVector3(9, 8, 7) }
            }
        });

        var shadow = canvas.GetShadow<TestSpatialShadow>(1);
        Assert.Equal(new FPVector3(9, 8, 7), shadow.position);
        Assert.Equal(new FP(5), shadow.scale);
    }

    // ============================================================
    // 端到端：ProjectorSystem → Pipeline → Transport → Canvas
    // ============================================================

    /// <summary>
    /// 完整链路：Logic 改字段 → Shadow 自动更新
    /// </summary>
    [Fact]
    public void EndToEnd_LogicChange_UpdatesShadow()
    {
        var stage = new Stage();
        var projector = new ProjectorSystem();
        projector.Assemble(stage, stage.sa);

        var canvas = new Canvas();
        canvas.Register<ProjectFieldInfo, TestSpatialShadow>();

        var pipeline = new ProjectionPipeline();
        pipeline.observers.Add(new Observer { type = ObserverType.Player });
        pipeline.transport = new LocalTransport { canvas = canvas };

        var info = stage.AddBehaviorInfo<ProjectFieldInfo>(1);
        info.position = new FPVector3(1, 2, 3);

        projector.EndTick();
        pipeline.Process(projector.packets);

        var shadow = canvas.GetShadow<TestSpatialShadow>(1);
        Assert.NotNull(shadow);
        Assert.Equal(new FPVector3(1, 2, 3), shadow.position);
    }

    /// <summary>
    /// 端到端：多帧增量同步
    /// </summary>
    [Fact]
    public void EndToEnd_MultiFrame_IncrementalSync()
    {
        var stage = new Stage();
        var projector = new ProjectorSystem();
        projector.Assemble(stage, stage.sa);

        var canvas = new Canvas();
        canvas.Register<ProjectFieldInfo, TestSpatialShadow>();

        var pipeline = new ProjectionPipeline();
        pipeline.observers.Add(new Observer { type = ObserverType.Player });
        pipeline.transport = new LocalTransport { canvas = canvas };

        var info = stage.AddBehaviorInfo<ProjectFieldInfo>(1);
        // 首帧全量
        projector.EndTick();
        pipeline.Process(projector.packets);
        var shadow = canvas.GetShadow<TestSpatialShadow>(1);
        Assert.NotNull(shadow);

        // 第二帧无脏
        projector.EndTick();
        pipeline.Process(projector.packets);

        // 第三帧改 scale
        info.scale = new FP(7);
        projector.EndTick();
        pipeline.Process(projector.packets);

        Assert.Equal(new FP(7), shadow.scale);
    }
}

/// <summary>
/// 测试用 Shadow — 对应 ProjectFieldInfo（position index 0, scale index 1）
/// </summary>
public class TestSpatialShadow : Shadow, IShadowApply<TestSpatialShadow>
{
    /// <summary>
    /// 位置
    /// </summary>
    public FPVector3 position { get; set; }
    /// <summary>
    /// 缩放
    /// </summary>
    public FP scale { get; set; } = FP.One;

    /// <summary>
    /// 应用脏字段 — 后续迁至 SG 生成
    /// </summary>
    internal static void ApplyTo(TestSpatialShadow shadow, ulong fieldmask, object[] values)
    {
        var i = 0;

        // Bit0: position
        if (0 != (fieldmask & 1)) shadow.position = (FPVector3)values[i++];

        // Bit1: scale
        if (0 != (fieldmask & (1ul << 1))) shadow.scale = (FP)values[i++];
    }

    static Action<TestSpatialShadow, ulong, object[]> IShadowApply<TestSpatialShadow>.ApplyTo => ApplyTo;
}
