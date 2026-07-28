using System;
using System.Diagnostics;
using System.Threading;
using Goblin.Common;
using Goblin.Core;
using Goblin.Debug;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors.Sa;
using Goblin.Gameplay.Logic.Common.BuildDatas;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Projection;
using Goblin.Gameplay.Projection.Core;
using Goblin.Gameplay.Projection.Rules;
using Goblin.Gameplay.Projection.Shadows;
using Goblin.Gameplay.Projection.Transport;
using Goblin.Gameplay.Render;
using Goblin.Gameplay.Logic.Core;
using Goblin.Sys.Common;
using Kowtow.Math;

namespace Goblin.Sys.Gameplay;

public class GameplayProxy : Proxy<GameplayModel>
{
    /// <summary>
    /// 逻辑场景
    /// </summary>
    public Stage stage { get; private set; }
    /// <summary>
    /// 输入系统
    /// </summary>
    public InputSystem input { get; private set; }
    /// <summary>
    /// 当前座位
    /// </summary>
    public ulong selfseat { get; set; }
    /// <summary>
    /// 数据画布
    /// </summary>
    public Canvas canvas { get; private set; }
    /// <summary>
    /// 投影管线
    /// </summary>
    private ProjectionPipeline pipeline;
    /// <summary>
    /// 逻辑步数计数器（用于定期清理频率规则等周期任务）
    /// </summary>
    private long stepcount;
    /// <summary>
    /// 逻辑 Step 耗时，单位毫秒
    /// </summary>
    public int stepms { get; private set; }

    /// <summary>
    /// 将管线产出的观察者包应用到 Canvas（调用方须在主线程）
    /// </summary>
    public void ApplyProjection()
    {
        if (null == pipeline || 0 == pipeline.observerpackets.Length) return;
        canvas?.ApplyPackets(pipeline.observerpackets);
    }
    /// <summary>
    /// 时间缩放（代理到 Stage.timescale）
    /// </summary>
    public float timescale => stage.timescale.AsFloat();

    public bool physdraw { get; set; } = false;
    public bool showinfo { get; set; } = false;
    public bool dancing { get; set; } = false;
    public bool enemyautopilot { get; set; } = false;

    /// <summary>
    /// 是否多线程
    /// </summary>
    private bool multithread;
    private Thread? thread;
    private volatile bool restoreing;
    private ulong lastseat;

    /// <summary>
    /// 创建游戏
    /// </summary>
    public void CreateGame(BuildData data, bool multithread = false)
    {
        if (null != stage)
        {
            stage.Stop();
            stage.Dispose();
        }

        this.multithread = multithread;
        // 兜底初始化 input，避免未调用 Initialize 时 NRE
        if (null == input) input = new InputSystem();
        selfseat = data.seat;
        stage = new Stage().Initialize(data.sdata);

        // 构建投影管线：ProjectorSystem → Pipeline（只出包，不自动传输）
        canvas = new Canvas();
        canvas.Register<SpatialInfo, SpatialShadow>();
        canvas.Register<HUDInfo, HUDShadow>();
        canvas.Register<FacadeInfo, FacadeShadow>();
        pipeline = new ProjectionPipeline();
        pipeline.transport = new LocalTransport { canvas = canvas };

        // Phase 1：注册 Player Observer，通过 ObserverFactory 组装裁剪链
        var heroactor = stage.seat.GetActor(selfseat);
        var crop = ObserverFactory.CreateRuleChain(ObserverType.Player, selfseat);

        // 注入 AOI 位置查询和可见性查询委托
        foreach (var rule in crop.GetRules())
        {
            if (rule is AOIRule aoi)
                aoi.positionlookup = canvas.TryGetPosition;
            else if (rule is VisibilityRule vis)
            {
                // TODO: Phase 2+ 替换为游戏可见性查询（隐身、战争迷雾等）
                // 当前用 HasActor 仅检查 Canvas 中是否存在数据
                vis.visibilitylookup = canvas.HasActor;
            }
            else if (rule is PermissionRule perm)
            {
                // TODO: Phase 2+ 由配置注册权限规则
                // 示例：perm.Add(ObserverType.Player, typeof(SpatialInfo), ulong.MaxValue);
            }
            else if (rule is FrequencyRule freq)
            {
                // TODO: Phase 2+ 由配置注册频率规则
                // 示例：freq.Add(typeof(SpatialInfo), 0, 3);  // 位置字段每 3 帧推送
            }
        }

        pipeline.observers.Add(new Observer
        {
            type = ObserverType.Player,
            id = selfseat,
            observedactor = heroactor,
            crop = crop,
        });

        // 网络 Type 注册（为后续 NetworkTransport 做准备）
        RegisterProjectionTypes();

        // 接入调试服务
        engine.debug.Attach(new GameplayStateProvider(stage), stage);

        if (false == multithread)
        {
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
            return;
        }

        int logicms = (int)GAME_DEFINE.LOGIC_TICK_MS;
        thread = new Thread(() =>
        {
            var sw = new Stopwatch();
            try
            {
                while (true)
                {
                    sw.Restart();
                    if (engine.debug.OnBeforeStep() == false) { Thread.Sleep(1); continue; }
                    OnStep();
                    stepms = (int)sw.ElapsedMilliseconds;
                    if (stepms < logicms) Thread.Sleep(logicms - stepms);
                }
            }
            catch (ThreadInterruptedException) { }
        });
        thread.Start();
    }

    /// <summary>
    /// 销毁游戏
    /// </summary>
    public void DestroyGame()
    {
        if (false == multithread)
        {
            engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
        }
        else
        {
            thread?.Interrupt();
            thread?.Join();
            thread = null;
        }

        engine.debug.Detach();
        canvas = null;
        pipeline = null;
        stage?.Dispose();
        stage = null;
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame() => stage?.Start();

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame() => stage?.Pause();

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame() => stage?.Resume();

    /// <summary>
    /// 停止游戏
    /// </summary>
    public void StopGame() => stage?.Stop();

    /// <summary>
    /// 快照
    /// </summary>
    public void Snapshot() => stage?.Snapshot();

    /// <summary>
    /// 恢复
    /// </summary>
    public void Restore()
    {
        restoreing = true;
    }

    /// <summary>
    /// 切换座位
    /// </summary>
    public void SwitchSeat(ulong seat)
    {
        selfseat = seat;
    }

    /// <summary>
    /// 卸载
    /// </summary>
    public void UnLoad()
    {
        DestroyGame();
    }

    /// <summary>
    /// 逻辑帧驱动
    /// </summary>
    private void OnStep()
    {
        // 恢复中直接执行恢复
        if (restoreing)
        {
            stage.Restore();
            restoreing = false;
            return;
        }

        if (null == stage || StageState.Ticking != stage.state) return;
        if (null == input) return;

        // 消费渲染层输入
        while (input.TryDequeueCommand(out var command))
        {
            stage.SetCommand(command);
            command.Reset();
            ObjectPool.Set(command);
        }

        var joystick = input.GetInput(INPUT_DEFINE.JOYSTICK);
        var ba = input.GetInput(INPUT_DEFINE.BA);
        var bb = input.GetInput(INPUT_DEFINE.BB);
        var bc = input.GetInput(INPUT_DEFINE.BC);

        var curseat = selfseat;
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

        // 投影管线：ProjectorSystem 出包 → 裁剪 → 传输 → Canvas
        var ps = stage.projector;
        if (null != ps && null != ps.packets)
        {
            pipeline?.Process(ps.packets);
        }

        // 定期清理频率规则过期条目（每 300 帧，约 10 秒 @30fps）
        stepcount++;
        if (0 == stepcount % 300) pipeline?.CleanupFrequencyRules(stepcount - 1000);
    }

    /// <summary>
    /// 敌人自动驾驶
    /// </summary>
    private void EnemyAutopoilot()
    {
        if (false == enemyautopilot) return;
        for (int i = 3; i <= 3 + 16; i++)
        {
            var ismove = engine.random.Range(0, 100) < 70;
            stage.PushInput((ulong)i, INPUT_DEFINE.JOYSTICK, ismove, new IntVector2(engine.random.Range(-1000, 1000), engine.random.Range(-1000, 1000)));

            var isba = engine.random.Range(0, 500) < 100;
            stage.PushInput((ulong)i, INPUT_DEFINE.BA, isba, new IntVector2());
        }
    }

    private void OnFixedTick(FixedTickEvent e)
    {
        stepms = (int)(e.tick * 1000);
        if (engine.debug.OnBeforeStep() == false) return;
        OnStep();
    }

    /// <summary>
    /// 注册投影类型到 NetworkTransport 类型表（为网络模式做准备）
    /// </summary>
    private static void RegisterProjectionTypes()
    {
        NetworkTransport.RegisterType(typeof(SpatialInfo));
        NetworkTransport.RegisterType(typeof(HUDInfo));
        NetworkTransport.RegisterType(typeof(FacadeInfo));
    }
}
