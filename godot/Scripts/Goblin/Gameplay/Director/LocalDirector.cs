using System.Collections.Generic;
using Goblin.Common;
using Goblin.Debug;
using Goblin.Gameplay.Director.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Projection;
using Kowtow.Math;

namespace Goblin.Gameplay.Director;

/// <summary>
/// 本地单机导演 — 驱动 Stage + RenderWorld，串联 ProjectorSystem 到 Transport 管线
/// </summary>
public class LocalDirector : GameplayDirector
{
    /// <summary>
    /// 是否渲染
    /// </summary>
    public override bool rendering
    {
        get
        {
            if (restoreing) return false;
            if (null == stage || StageState.Ticking != stage.state) return false;
            return true;
        }
    }

    /// <summary>
    /// 是否正在恢复，volatile 保证逻辑线程与主线程可见性
    /// </summary>
    private volatile bool restoreing = false;
    /// <summary>
    /// 同步锁对象
    /// </summary>
    private readonly object @lock = new();
    /// <summary>
    /// 上一帧座位号，检测座位切换
    /// </summary>
    private ulong lastseat { get; set; }
    /// <summary>
    /// 碰撞盒列表
    /// </summary>
    private readonly List<ColliderInfo> colliders = new();

    protected override void OnCreateGame()
    {
        // 接入调试服务
        engine.debug.Attach(new GameplayStateProvider(stage), stage);
        onbeforestep = engine.debug.OnBeforeStep;
        onbeforetick = engine.debug.OnBeforeTick;
    }

    protected override void OnDestroyGame()
    {
        engine.debug.Detach();
        onbeforestep = null;
        onbeforetick = null;
    }

    protected override void OnStartGame() => stage.Start();
    protected override void OnPauseGame() => stage.Pause();
    protected override void OnResumeGame() => stage.Resume();
    protected override void OnStopGame() => stage.Stop();

    protected override void OnSnapshot()
    {
        stage.Snapshot();
    }

    protected override void OnRestore()
    {
        restoreing = true;
    }

    /// <summary>
    /// 渲染帧驱动 — 当前 Phase 1 投影在 Stage.Step() 内自动完成，此处预留 Phase 2 插值
    /// </summary>
    protected override void OnTick()
    {
        if (restoreing) return;
        // Phase 2+：在此驱动 RenderWorld 的插值/预测
    }

    /// <summary>
    /// 逻辑帧驱动
    /// </summary>
    protected override void OnStep()
    {
        // 恢复中直接执行恢复
        if (restoreing)
        {
            stage.Restore();
            restoreing = false;
            return;
        }

        if (null == stage || StageState.Ticking != stage.state) return;

        // 消费渲染层输入
        while (world.input.TryDequeueCommand(out var command))
        {
            stage.SetCommand(command);
            command.Reset();
            ObjectPool.Set(command);
        }

        var joystick = world.input.GetInput(INPUT_DEFINE.JOYSTICK);
        var ba = world.input.GetInput(INPUT_DEFINE.BA);
        var bb = world.input.GetInput(INPUT_DEFINE.BB);
        var bc = world.input.GetInput(INPUT_DEFINE.BC);

        var curseat = world.selfseat;
        if (lastseat != curseat)
        {
            lastseat = curseat;
        }

        stage.PushInput(lastseat, INPUT_DEFINE.JOYSTICK, joystick.press, joystick.dire);
        stage.PushInput(lastseat, INPUT_DEFINE.BA, ba.press, ba.dire);
        stage.PushInput(lastseat, INPUT_DEFINE.BB, bb.press, bb.dire);
        stage.PushInput(lastseat, INPUT_DEFINE.BC, bc.press, bc.dire);

        // TODO 硬编码技能映射，后续由 SkillKeys 配置驱动
        if (bb.press) stage.PushSkillFrame(lastseat, 10010);
        if (bc.press) stage.PushSkillFrame(lastseat, 10020);

        EnemyAutopoilot();
        stage.Step();

        lock (@lock)
        {
            colliders.Clear();
            if (false == stage.SeekBehaviorInfos(out List<ColliderInfo> infos)) return;
            colliders.AddRange(infos);
        }
    }

    /// <summary>
    /// 敌人自动驾驶
    /// </summary>
    private void EnemyAutopoilot()
    {
        if (false == engine.proxy.gameplay.enemyautopilot) return;
        for (int i = 3; i <= 3 + 16; i++)
        {
            var ismove = engine.random.Range(0, 100) < 70;
            stage.PushInput((ulong)i, INPUT_DEFINE.JOYSTICK, ismove, new IntVector2(engine.random.Range(-1000, 1000), engine.random.Range(-1000, 1000)));

            var isba = engine.random.Range(0, 500) < 100;
            stage.PushInput((ulong)i, INPUT_DEFINE.BA, isba, new IntVector2());
        }
    }
}
