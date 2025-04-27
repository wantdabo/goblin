using Queen.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Common;

/// <summary>
/// Tick 事件
/// </summary>
public struct TickEvent : IEvent
{
    /// <summary>
    /// 帧号
    /// </summary>
    public uint frame;
    /// <summary>
    /// 流逝的时间 s/秒
    /// </summary>
    public float tick;
}

/// <summary>
/// LateTick 事件
/// </summary>
public struct LateTickEvent : IEvent
{
    /// <summary>
    /// 帧号
    /// </summary>
    public uint frame;
    /// <summary>
    /// 流逝的时间 s/秒
    /// </summary>
    public float tick;
}

/// <summary>
/// Ticker, Tick 驱动组件
/// </summary>
public class Ticker : Comp
{
    /// <summary>
    /// 事件订阅/派发者
    /// </summary>
    public Eventor eventor;

    /// <summary>
    /// 帧数
    /// </summary>
    public uint frame { get; private set; }
    /// <summary>
    /// tick 间隔 (50 fps)
    /// </summary>
    public float tick { get; private set; } = 1 / 50f;
    /// <summary>
    /// 最新的毫秒
    /// </summary>
    private long nowMilliSeconds { get { return DateTimeOffset.Now.ToUnixTimeMilliseconds(); } }
    /// <summary>
    /// 上一次 Execute 记录的毫秒
    /// </summary>
    private long lastMilliSeconds = 0;
    /// <summary>
    /// 时间流逝溢出记录
    /// </summary>
    private float elapsed = 0f;

    protected override void OnCreate()
    {
        base.OnCreate();
        engine.eventor.Listen<ExecuteEvent>(OnExecute);
        eventor = AddComp<Eventor>();
        eventor.Create();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        engine.eventor.UnListen<ExecuteEvent>(OnExecute);
    }

    public void OnExecute(ExecuteEvent e)
    {
        if (0 == lastMilliSeconds)
        {
            lastMilliSeconds = nowMilliSeconds;

            return;
        }

        var nms = nowMilliSeconds;
        elapsed += (nms - lastMilliSeconds) / Config.thousand;
        lastMilliSeconds = nms;
        while (elapsed >= tick)
        {
            frame++;
            // 驱动计时器
            TickTimerInfos(timerInfos, tick);
            // 派发事件，时间流逝
            eventor.Tell(new TickEvent { frame = frame, tick = tick });
            eventor.Tell(new LateTickEvent { frame = frame, tick = tick });
            elapsed -= tick;
        }
    }

    #region Timer
    /// <summary>
    /// 计时器结构体
    /// </summary>
    private struct TimerInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public uint id;
        /// <summary>
        /// Callback/回调
        /// </summary>
        public Action<float> action;
        /// <summary>
        /// 触发所需的时间
        /// </summary>
        public float duration;
        /// <summary>
        /// 当前过去了多少时间
        /// </summary>
        public float elapsed;
        /// <summary>
        /// 循环次数（设置负数为将会一直循环，例如 -1）
        /// </summary>
        public int loop;
    }

    /// <summary>
    /// 自增的 TimerID
    /// </summary>
    private uint timerIncrementId = 0;
    private List<TimerInfo> timerInfos = new();

    /// <summary>
    /// 停止计时器
    /// </summary>
    /// <param name="id">计时器 ID</param>
    /// <param name="tickDef">计时器类型</param>
    public void StopTimer(uint id)
    {
        engine.EnsureThread();
        if (0 == timerInfos.Count) return;

        for (int i = timerInfos.Count - 1; i >= 0; i--)
        {
            var info = timerInfos[i];
            if (id != info.id) continue;

            timerInfos.RemoveAt(i);
            break;
        }
    }

    /// <summary>
    /// 开始计时
    /// </summary>
    /// <param name="action">回调</param>
    /// <param name="duration">触发所需的时间</param>
    /// <param name="loop">循环次数（设置负数为将会一直循环，例如 -1）</param>
    /// <returns>计时器 ID</returns>
    public uint Timing(Action<float> action, float duration, int loop)
    {
        engine.EnsureThread();
        timerIncrementId++;

        TimerInfo info = new()
        {
            id = timerIncrementId,
            action = action,
            duration = duration,
            loop = loop
        };
        timerInfos.Add(info);

        return info.id;
    }

    private void TickTimerInfos(List<TimerInfo> infos, float tick)
    {
        if (0 == infos.Count) return;

        for (int i = infos.Count - 1; i >= 0; i--)
        {
            var info = infos[i];
            info.elapsed += tick;
            infos[i] = info;
            if (info.duration > info.elapsed) continue;

            info.elapsed = Math.Max(0, info.elapsed - info.duration);
            info.loop--;
            infos[i] = info;
            if (0 == info.loop) infos.RemoveAt(i);

            info.action.Invoke(tick);
        }
    }
    #endregion
}
