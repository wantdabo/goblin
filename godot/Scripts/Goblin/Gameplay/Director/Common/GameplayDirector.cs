using System;
using System.Diagnostics;
using System.Threading;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.BuildDatas;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Projection;
using Kowtow.Math;

namespace Goblin.Gameplay.Director.Common;

/// <summary>
/// 导演 — 指挥游戏运行，驱动逻辑层 Stage 和表现层 RenderWorld
/// 新 Property Sync 架构：ProjectorSystem → ProjectionPipeline → Transport → RenderWorld
/// </summary>
public abstract class GameplayDirector : Comp
{
    /// <summary>
    /// 是否渲染，驱动 RenderWorld
    /// </summary>
    public abstract bool rendering { get; }
    /// <summary>
    /// 游戏数据
    /// </summary>
    protected BuildData data { get; private set; }
    /// <summary>
    /// 表现世界
    /// </summary>
    public RenderWorld renderworld { get; set; }
    /// <summary>
    /// 逻辑场景
    /// </summary>
    protected Stage stage { get; set; }
    /// <summary>
    /// 是否多线程
    /// </summary>
    public bool multithread { get; private set; }
    /// <summary>
    /// 子线程
    /// </summary>
    private Thread? thread { get; set; }
    /// <summary>
    /// 逻辑 Step 耗时，单位毫秒
    /// </summary>
    public int stepms { get; private set; }
    /// <summary>
    /// 逻辑帧前钩子，返回 false 跳过本帧 OnStep
    /// </summary>
    public Func<bool>? onbeforestep { get; set; }
    /// <summary>
    /// 渲染帧前钩子，返回 false 跳过本帧 OnTick
    /// </summary>
    public Func<bool>? onbeforetick { get; set; }
    /// <summary>
    /// 时间缩放
    /// </summary>
    public float timescale => stage.timescale.AsFloat();
    /// <summary>
    /// 游戏世界（提供 input/selfseat 等）
    /// </summary>
    public GameplayDirector world => this;
    /// <summary>
    /// 输入系统
    /// </summary>
    public InputSystem input { get; private set; }
    /// <summary>
    /// 当前座位
    /// </summary>
    public ulong selfseat { get; set; }

    /// <summary>
    /// 初始化世界
    /// </summary>
    public GameplayDirector Initialize(ulong seat)
    {
        input = new InputSystem();
        selfseat = seat;
        return this;
    }

    /// <summary>
    /// 切换座位
    /// </summary>
    public void SwitchSeat(ulong seat)
    {
        selfseat = seat;
    }

    /// <summary>
    /// 创建游戏
    /// </summary>
    /// <param name="data">游戏数据</param>
    /// <param name="multithread">是否多线程</param>
    public void CreateGame(BuildData data, bool multithread = false)
    {
        this.data = data;
        this.multithread = multithread;
        stage = new Stage().Initialize(data.sdata);
        renderworld = new RenderWorld();
        // 注册 BehaviorInfo → Component 映射
        renderworld.RegisterMapping<SpatialInfo, SpatialComponent>();
        // 连接投影管线：LocalTransport → RenderWorld
        var transport = stage.projectorpipeline.transport as LocalTransport;
        if (null != transport) transport.renderworld = renderworld;
        OnCreateGame();

        engine.ticker.eventor.Listen<TickEvent>(OnRenderTick);
        if (false == multithread)
        {
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
            return;
        }
        thread = new Thread(() =>
        {
            int logicms = (int)GAME_DEFINE.LOGIC_TICK_MS;
            var sw = new Stopwatch();
            try
            {
                while (true)
                {
                    sw.Restart();
                    if (onbeforestep?.Invoke() == false) { Thread.Sleep(1); continue; }
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
        engine.ticker.eventor.UnListen<TickEvent>(OnRenderTick);
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

        OnDestroyGame();
        stage.Dispose();
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        OnStartGame();
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        OnPauseGame();
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        OnResumeGame();
    }

    /// <summary>
    /// 停止游戏
    /// </summary>
    public void StopGame()
    {
        OnStopGame();
    }

    /// <summary>
    /// 快照
    /// </summary>
    public void Snapshot()
    {
        OnSnapshot();
    }

    /// <summary>
    /// 恢复
    /// </summary>
    public void Restore()
    {
        OnRestore();
    }

    protected void OnRenderTick(TickEvent e)
    {
        if (onbeforetick?.Invoke() == false) return;
        if (false == rendering) return;
        OnTick();
    }

    protected void OnFixedTick(FixedTickEvent e)
    {
        stepms = (int)(e.tick * 1000);
        if (onbeforestep?.Invoke() == false) return;
        OnStep();
    }

    /// <summary>
    /// 创建游戏钩子
    /// </summary>
    protected abstract void OnCreateGame();
    /// <summary>
    /// 销毁游戏钩子
    /// </summary>
    protected abstract void OnDestroyGame();
    /// <summary>
    /// 开始游戏钩子
    /// </summary>
    protected abstract void OnStartGame();
    /// <summary>
    /// 暂停游戏钩子
    /// </summary>
    protected abstract void OnPauseGame();
    /// <summary>
    /// 恢复游戏钩子
    /// </summary>
    protected abstract void OnResumeGame();
    /// <summary>
    /// 停止游戏钩子
    /// </summary>
    protected abstract void OnStopGame();
    /// <summary>
    /// 快照钩子
    /// </summary>
    protected abstract void OnSnapshot();
    /// <summary>
    /// 恢复钩子
    /// </summary>
    protected abstract void OnRestore();
    /// <summary>
    /// 渲染层驱动，单线程
    /// </summary>
    protected abstract void OnTick();
    /// <summary>
    /// 逻辑层驱动，根据配置决定单线程或多线程
    /// </summary>
    protected abstract void OnStep();
}
