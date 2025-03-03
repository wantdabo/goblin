﻿using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Kowtow.Math;
using System;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Common
{
    /// <summary>
    /// FPTick 事件
    /// </summary>
    public struct FPTickEvent : IEvent
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 流逝的时间 s/秒
        /// </summary>
        public FP tick { get; set; }
    }

    /// <summary>
    /// FPLateTick 事件
    /// </summary>
    public struct FPLateTickEvent : IEvent
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 流逝的时间 s/秒
        /// </summary>
        public FP tick { get; set; }
    }

    /// <summary>
    /// 确定性 Ticker, 逻辑层 Tick 驱动组件
    /// </summary>
    public class FPTicker : Comp
    {
        /// <summary>
        /// 事件订阅/派发者
        /// </summary>
        public Eventor eventor { get; set; }
        /// <summary>
        /// 跳帧
        /// </summary>
        public uint breakframes { get; set; }
        private uint mFrame = 0;
        /// <summary>
        /// 最新的帧号
        /// </summary>
        public uint frame
        {
            get { return mFrame; }
            private set { mFrame = value; }
        }
        /// <summary>
        /// 最新的流逝时间 s/秒
        /// </summary>
        public FP tick { get; private set; } = GAME_DEFINE.LOGIC_TICK;

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
        }

        /// <summary>
        /// 流逝时间
        /// </summary>
        public void Tick()
        {
            // 跳帧
            if (breakframes > 0)
            {
                breakframes--;
                return;
            }

            frame++;
            // 驱动计时器
            TickTimerInfos();
            // 派发事件，时间流逝
            eventor.Tell(new FPTickEvent { frame = frame, tick = tick });
            eventor.Tell(new FPLateTickEvent { frame = frame, tick = tick });
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
            public uint id { get; set; }
            /// <summary>
            /// Callback/回调
            /// </summary>
            public Action<FP> action { get; set; }
            /// <summary>
            /// 触发所需的时间
            /// </summary>
            public FP duration { get; set; }
            /// <summary>
            /// 当前过去了多少时间
            /// </summary>
            public FP elapsed { get; set; }
            /// <summary>
            /// 循环次数（设置负数为将会一直循环，例如 -1）
            /// </summary>
            public int loop { get; set; }
        }

        /// <summary>
        /// 自增的 TimerID
        /// </summary>
        private uint timerIncrementId { get; set; } = 0;
        private Dictionary<uint, TimerInfo> timerdict = new();
        private Queue<uint> recyTimers = new();

        /// <summary>
        /// 停止计时器
        /// </summary>
        /// <param name="id">计时器 ID</param>
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
        public uint Timing(Action<FP> action, FP duration, int loop)
        {
            timerIncrementId++;

            TimerInfo info = new()
            {
                id = timerIncrementId,
                action = action,
                duration = duration,
                loop = loop
            };
            timerdict.Add(info.id, info);

            return info.id;
        }

        private List<TimerInfo> infoTemps = new();
        private List<uint> timerTemps = new();

        private void TickTimerInfos()
        {
            while (recyTimers.TryDequeue(out var tid)) timerdict.Remove(tid);

            if (0 == timerdict.Count) return;
            infoTemps.Clear();
            timerTemps.Clear();
            foreach (var kv in timerdict)
            {
                var info = kv.Value;
                info.elapsed += tick;
                infoTemps.Add(info);
                if (info.duration > info.elapsed) continue;

                info.elapsed = FPMath.Max(0, info.elapsed - info.duration);
                info.loop--;
                infoTemps.Add(info);

                timerTemps.Add(info.id);
            }

            foreach (var infoTemp in infoTemps)
            {
                timerdict.Remove(infoTemp.id);
                timerdict.Add(infoTemp.id, infoTemp);
            }

            foreach (var timer in timerTemps)
            {
                if (timerdict.TryGetValue(timer, out var action) && false == recyTimers.Contains(action.id))
                {
                    action.action.Invoke(tick);
                    if (0 == action.loop) StopTimer(action.id);
                }
            }
        }
        #endregion
    }
}
