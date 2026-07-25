using Goblin.Gameplay.Logic.Behaviors.Sa;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Projection;
using Goblin.Logic.Standalone.TestFixtures;
using Kowtow.Math;

namespace Goblin.Logic.Tests;

/// <summary>
/// T1.9 RenderWorld / Entity / Component 测试 + 端到端投影链路
/// </summary>
[Collection("GBL")]
public class RenderWorldTests
{
    // ============================================================
    // RenderWorld / Entity / Component
    // ============================================================

    /// <summary>
    /// Apply 创建 Entity + Component，写入字段
    /// </summary>
    [Fact]
    public void Apply_CreatesEntityAndComponent()
    {
        var world = new RenderWorld();
        world.RegisterMapping<ProjectFieldInfo, TestSpatialComponent>();

        world.Apply(1, typeof(ProjectFieldInfo), 0, 0, 3, new object[] { new FPVector3(1, 2, 3), new FP(5) });

        var entity = world.GetEntity(1);
        Assert.NotNull(entity);
        var comp = entity.GetComp<TestSpatialComponent>();
        Assert.NotNull(comp);
        Assert.Equal(new FPVector3(1, 2, 3), comp.position);
        Assert.Equal(new FP(5), comp.scale);
    }

    /// <summary>
    /// 无映射的 BehaviorInfo 不创建 Component（但创建 Entity）
    /// </summary>
    [Fact]
    public void Apply_NoMapping_NoComponent()
    {
        var world = new RenderWorld();

        world.Apply(1, typeof(ProjectFieldInfo), 0, 0, 3, new object[] { new FPVector3(1, 2, 3), FP.One });

        var entity = world.GetEntity(1);
        Assert.NotNull(entity);
        Assert.Null(entity.GetComp<TestSpatialComponent>());
    }

    /// <summary>
    /// RmvEntity 销毁实体
    /// </summary>
    [Fact]
    public void RmvEntity_RemovesEntity()
    {
        var world = new RenderWorld();
        world.RegisterMapping<ProjectFieldInfo, TestSpatialComponent>();
        world.Apply(1, typeof(ProjectFieldInfo), 0, 0, 3, new object[] { new FPVector3(1, 2, 3), FP.One });

        world.RmvEntity(1);

        Assert.Null(world.GetEntity(1));
    }

    /// <summary>
    /// 部分字段 Apply 只更新对应位
    /// </summary>
    [Fact]
    public void Apply_PartialMask_UpdatesPartial()
    {
        var world = new RenderWorld();
        world.RegisterMapping<ProjectFieldInfo, TestSpatialComponent>();
        world.Apply(1, typeof(ProjectFieldInfo), 0, 0, 3, new object[] { new FPVector3(1, 2, 3), new FP(5) });

        // 只更新 position（位 0）
        world.Apply(1, typeof(ProjectFieldInfo), 0, 0, 1, new object[] { new FPVector3(9, 8, 7) });

        var comp = world.GetEntity(1).GetComp<TestSpatialComponent>();
        Assert.Equal(new FPVector3(9, 8, 7), comp.position);
        Assert.Equal(new FP(5), comp.scale);
    }

    // ============================================================
    // 端到端：ProjectorSystem → Pipeline → Transport → RenderWorld
    // ============================================================

    /// <summary>
    /// 完整链路：Logic 改字段 → Component 自动更新
    /// </summary>
    [Fact]
    public void EndToEnd_LogicChange_UpdatesComponent()
    {
        var stage = new Stage();
        var projector = new ProjectorSystem();
        projector.Assemble(stage, stage.sa);

        var world = new RenderWorld();
        world.RegisterMapping<ProjectFieldInfo, TestSpatialComponent>();

        var pipeline = new ProjectionPipeline();
        pipeline.observers.Add(new Observer { type = ObserverType.Player });
        pipeline.transport = new LocalTransport { renderworld = world };

        var info = stage.AddBehaviorInfo<ProjectFieldInfo>(1);
        info.position = new FPVector3(1, 2, 3);

        projector.EndTick();
        pipeline.Process(projector.packets);

        var comp = world.GetEntity(1).GetComp<TestSpatialComponent>();
        Assert.NotNull(comp);
        Assert.Equal(new FPVector3(1, 2, 3), comp.position);
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

        var world = new RenderWorld();
        world.RegisterMapping<ProjectFieldInfo, TestSpatialComponent>();

        var pipeline = new ProjectionPipeline();
        pipeline.observers.Add(new Observer { type = ObserverType.Player });
        pipeline.transport = new LocalTransport { renderworld = world };

        var info = stage.AddBehaviorInfo<ProjectFieldInfo>(1);
        // 首帧全量
        projector.EndTick();
        pipeline.Process(projector.packets);
        var comp = world.GetEntity(1).GetComp<TestSpatialComponent>();
        Assert.NotNull(comp);

        // 第二帧无脏
        projector.EndTick();
        pipeline.Process(projector.packets);

        // 第三帧改 scale
        info.scale = new FP(7);
        projector.EndTick();
        pipeline.Process(projector.packets);

        Assert.Equal(new FP(7), comp.scale);
    }

    /// <summary>
    /// 端到端：OnEntityCreated 事件触发
    /// </summary>
    [Fact]
    public void EndToEnd_OnEntityCreated_Fires()
    {
        var stage = new Stage();
        var projector = new ProjectorSystem();
        projector.Assemble(stage, stage.sa);

        var world = new RenderWorld();
        world.RegisterMapping<ProjectFieldInfo, TestSpatialComponent>();

        Entity created = null;
        world.OnEntityCreated += e => created = e;

        var pipeline = new ProjectionPipeline();
        pipeline.observers.Add(new Observer { type = ObserverType.Player });
        pipeline.transport = new LocalTransport { renderworld = world };

        stage.AddBehaviorInfo<ProjectFieldInfo>(1);
        projector.EndTick();
        pipeline.Process(projector.packets);

        Assert.NotNull(created);
        Assert.Equal(1ul, created.actor);
    }
}

/// <summary>
/// 测试用 Component — 对应 ProjectFieldInfo（position index 0, scale index 1）
/// </summary>
public class TestSpatialComponent : Component
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
    /// 按 fieldmask 写入字段，values 按 index 顺序消费
    /// </summary>
    public override void Apply(ulong fieldmask, object[] values)
    {
        var i = 0;
        if (0ul != (fieldmask & (1ul << 0))) { position = (FPVector3)values[i]; i++; }
        if (0ul != (fieldmask & (1ul << 1))) { scale = (FP)values[i]; i++; }
    }
}
