using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.Common
{
    /// <summary>
    /// Tick 事件
    /// </summary>
    public struct TickEvent : IEvent
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 流逝的时间 s/秒
        /// </summary>
        public float tick { get; set; }
    }

    /// <summary>
    /// LateTick 事件
    /// </summary>
    public struct LateTickEvent : IEvent
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 流逝的时间 s/秒
        /// </summary>
        public float tick { get; set; }
    }

    /// <summary>
    /// FixedTick 事件
    /// </summary>
    public struct FixedTickEvent : IEvent
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 流逝的时间 s/秒
        /// </summary>
        public float tick { get; set; }
    }

    /// <summary>
    /// FixedLateTick 事件
    /// </summary>
    public struct FixedLateTickEvent : IEvent
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
    /// Tick 类型
    /// </summary>
    public enum TickType
    {
        /// <summary>
        /// Tick 驱动
        /// </summary>
        Tick,
        /// <summary>
        /// Fixed 驱动
        /// </summary>
        FixedTick,
    }

    /// <summary>
    /// Ticker, 逻辑层 Tick 驱动组件
    /// </summary>
    public class Ticker : Comp
    {
        /// <summary>
        /// 事件订阅/派发者
        /// </summary>
        public Eventor eventor { get; set; }

        /// <summary>
        /// 时间缩放
        /// </summary>
        public float timeScale = 1f;

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private uint mFrame = 0;
        /// <summary>
        /// 最新的帧号
        /// </summary>
        public uint frame
        {
            get { return mFrame; }
            private set { mFrame = value; }
        }

        private float mTick = 0;
        /// <summary>
        /// 最新的流逝时间 s/秒
        /// </summary>
        public float tick
        {
            get { return mTick; }
            private set { mTick = value; }
        }

        private uint mFixedFrame = 0;
        /// <summary>
        /// Fixed 最新的帧号
        /// </summary>
        public uint fixedFrame
        {
            get { return mFixedFrame; }
            private set { mFixedFrame = value; }
        }

        private float mFixedTick = 0;
        /// <summary>
        /// Fixed 最新的流逝时间 s/秒
        /// </summary>
        public float fixedTick
        {
            get { return mFixedTick; }
            private set { mFixedTick = value; }
        }

        /// <summary>
        /// 流逝时间
        /// </summary>
        /// <param name="t">s/秒</param>
        public void Tick(float t)
        {
            tick = t * timeScale;
            frame++;
            // 驱动计时器
            TickTimerInfos(tick);
            // 派发事件，时间流逝
            eventor.Tell(new TickEvent { frame = frame, tick = tick });
            eventor.Tell(new LateTickEvent { frame = frame, tick = tick });
        }

        /// <summary>
        /// Fixed 流逝时间
        /// </summary>
        /// <param name="ft">s/秒</param>
        public void FixedTick(float ft)
        {
            var elapsed = ft * timeScale;
            if (elapsed < ft) ft = elapsed;

            while (true)
            {
                if (elapsed <= 0) break;
                fixedTick = elapsed > ft ? ft : elapsed;
                elapsed -= ft;

                fixedFrame++;
                // 派发事件，时间流逝
                eventor.Tell(new FixedTickEvent { frame = fixedFrame, tick = fixedTick });
                eventor.Tell(new FixedLateTickEvent { frame = fixedFrame, tick = fixedTick });
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
        private Dictionary<uint, TimerInfo> timerDict = new();
        private Queue<uint> recyTimers = new();

        /// <summary>
        /// 停止计时器
        /// </summary>
        /// <param name="id">计时器 ID</param>
        /// <param name="tickDef">计时器类型</param>
        public void StopTimer(uint id)
        {
            if (recyTimers.Contains(id)) return;
            recyTimers.Enqueue(id);
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
            timerIncrementId++;

            TimerInfo info = new()
            {
                id = timerIncrementId,
                action = action,
                duration = duration,
                loop = loop
            };
            timerDict.Add(info.id, info);

            return info.id;
        }

        private List<TimerInfo> infoTemps = new();
        private List<uint> timerTemps = new();
        private void TickTimerInfos(float tick)
        {
            while (recyTimers.TryDequeue(out var tid)) timerDict.Remove(tid);
            if (0 == timerDict.Count) return;
            
            foreach (var kv in timerDict)
            {
                var info = kv.Value;
                info.elapsed += tick;
                infoTemps.Add(info);
                if (info.duration > info.elapsed) continue;

                info.elapsed = Mathf.Max(0, info.elapsed - info.duration);
                info.loop--;
                infoTemps.Add(info);

                timerTemps.Add(info.id);
            }

            foreach (var infoTemp in infoTemps)
            {
                timerDict.Remove(infoTemp.id);
                timerDict.Add(infoTemp.id, infoTemp);
            }

            foreach (var timer in timerTemps)
            {
                if (timerDict.TryGetValue(timer, out var action) && false == recyTimers.Contains(action.id))
                {
                    action.action.Invoke(tick);
                    if (0 == action.loop) StopTimer(action.id);
                }
            }
            
            infoTemps.Clear();
            timerTemps.Clear();
        }
        #endregion
    }
}
