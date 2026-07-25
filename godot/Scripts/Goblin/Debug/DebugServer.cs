using System;
using System.Collections.Concurrent;
using System.Text.Json.Nodes;
using Goblin.Core;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Debug;

/// <summary>
/// 调试服务器 Comp——挂载在 Engine 上，随 Engine 生命周期。支持：
/// - 暂停/恢复/步进逻辑帧
/// - 断点条件（状态变更、属性阈值、帧号）
/// - 模拟输入注入
/// - 暂停/恢复渲染帧
/// - HTTP 状态查询
///
/// 当游戏未运行时，HTTP 仍可访问但状态接口返回空。
/// Director 通过 Attach/Detach 注册/注销运行时状态。
/// </summary>
public class DebugServer : Comp
{
    private volatile bool paused;
    private volatile bool renderingpaused;
    private volatile int stepcount;
    private Breakpoint? breakpoint { get; set; }

    /// <summary>当前游戏状态提供者（Attach 后有效，Detach 后为 null）</summary>
    internal IStateProvider? stateprovider { get; private set; }

    /// <summary>当前游戏逻辑场景（Attach 后有效，Detach 后为 null）</summary>
    private Stage? stage { get; set; }

    private DebugHttpServer? http { get; set; }

    /// <summary>待注入的模拟输入队列</summary>
    private readonly ConcurrentQueue<SimulatedInput> pendinginputs = new();

    protected override void OnCreate()
    {
        http = new DebugHttpServer(this, 9876);
        http.Start();
    }

    protected override void OnDestroy()
    {
        http?.Stop();
    }

    /// <summary>
    /// Director 创建游戏时调用，注册当前游戏的状态提供者和 Stage。
    /// </summary>
    public void Attach(IStateProvider provider, Stage s)
    {
        stateprovider = provider;
        stage = s;
    }

    /// <summary>
    /// Director 销毁游戏时调用，清除状态并重置调试控制。
    /// </summary>
    public void Detach()
    {
        stateprovider = null;
        stage = null;
        paused = false;
        renderingpaused = false;
        stepcount = 0;
        breakpoint = null;
        while (pendinginputs.TryDequeue(out _)) { }
    }

    /// <summary>
    /// 挂载到游戏主循环前。返回 false 则跳过本帧 OnStep。
    /// </summary>
    public bool OnBeforeStep()
    {
        // 注入待处理的模拟输入
        InjectPendingInputs();

        // 检查断点
        if (null != breakpoint && null != stateprovider && breakpoint.Evaluate(stateprovider))
        {
            paused = true;
            breakpoint = null;
        }

        // 步进模式
        if (0 < stepcount)
        {
            stepcount--;
            return true;
        }

        // 暂停
        if (paused) return false;

        return true;
    }

    /// <summary>
    /// 挂载到渲染帧前。返回 false 则跳过本帧 OnTick。
    /// </summary>
    public bool OnBeforeTick() => false == renderingpaused;

    public void Pause() { stepcount = 0; paused = true; }
    public void Resume() => paused = false;
    public void Step(int n) { stepcount = Math.Max(1, n); }
    public void PauseRender() => renderingpaused = true;
    public void ResumeRender() => renderingpaused = false;
    public void SetBreakpoint(Breakpoint bp) => breakpoint = bp;
    public void ClearBreakpoint() => breakpoint = null;

    public void InjectInput(SimulatedInput input) => pendinginputs.Enqueue(input);

    public JsonObject GetStatus()
    {
        JsonObject status = new()
        {
            ["paused"] = paused,
            ["rendering_paused"] = renderingpaused,
            ["step_count"] = stepcount,
            ["breakpoint"] = breakpoint?.type.ToString(),
        };

        if (null != stage)
        {
            status["frame"] = stage.frame;
            status["elapsed"] = GameplayStateProvider.FpToFloat(stage.elapsed);
            StageInfo? stageinfo = stage.GetBehaviorInfo<StageInfo>(stage.sa);
            status["actors"] = null != stageinfo ? stageinfo.actors.Count - 1 : 0;
        }
        else
        {
            status["frame"] = 0;
            status["elapsed"] = 0;
            status["actors"] = 0;
        }

        return status;
    }

    /// <summary>
    /// 在逻辑帧前注入所有待处理的模拟输入。
    /// </summary>
    private void InjectPendingInputs()
    {
        if (null == stage) return;

        while (pendinginputs.TryDequeue(out SimulatedInput? input))
        {
            ushort type = input.type switch
            {
                "JOYSTICK" => Gameplay.Logic.Common.Defines.INPUT_DEFINE.JOYSTICK,
                "BA" => Gameplay.Logic.Common.Defines.INPUT_DEFINE.BA,
                "BB" => Gameplay.Logic.Common.Defines.INPUT_DEFINE.BB,
                "BC" => Gameplay.Logic.Common.Defines.INPUT_DEFINE.BC,
                _ => (ushort)0,
            };
            if (0 == type) continue;

            stage.PushInput(input.seat, type, input.pressed,
                new Kowtow.Math.IntVector2(input.direx, input.direy));

            // 模拟 LocalDirector 的按键→技能映射
            if (input.pressed && type == Gameplay.Logic.Common.Defines.INPUT_DEFINE.BB)
                stage.PushSkillFrame(input.seat, 10010);
            if (input.pressed && type == Gameplay.Logic.Common.Defines.INPUT_DEFINE.BC)
                stage.PushSkillFrame(input.seat, 10020);
        }
    }
}
