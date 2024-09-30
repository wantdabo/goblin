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
    /// FixedTick 事件
    /// </summary>
    public struct FixedTickEvent : IEvent
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint fixedFrame;
        /// <summary>
        /// 流逝的时间 s/秒
        /// </summary>
        public float fixedTick;
    }

    /// <summary>
    /// FixedLateTick 事件
    /// </summary>
    public struct FixedLateTickEvent : IEvent
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint fixedFrame;
        /// <summary>
        /// 流逝的时间 s/秒
        /// </summary>
        public float fixedTick;
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
        /// 固定的流逝时间 s/秒
        /// </summary>
        public float unscaleFixedTick = 0.0166f;

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
            TickTimerInfos(TickType.Tick, tick);
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
                // 驱动 Fixed 计时器
                TickTimerInfos(TickType.FixedTick, fixedTick);
                // 派发事件，时间流逝
                eventor.Tell(new FixedTickEvent { fixedFrame = fixedFrame, fixedTick = fixedTick });
                eventor.Tell(new FixedLateTickEvent { fixedFrame = fixedFrame, fixedTick = fixedTick });
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
        private Dictionary<uint, TimerInfo> fixedTimerDict = new();
        private Queue<uint> recyFixedTimers = new();

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
        /// 停止 Fixed 计时器
        /// </summary>
        /// <param name="id">计时器 ID</param>
        /// <param name="tickDef">计时器类型</param>
        public void StopFixedTimer(uint id)
        {
            if (recyFixedTimers.Contains(id)) return;
            recyFixedTimers.Enqueue(id);
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

        /// <summary>
        /// 开始 Fixed 计时
        /// </summary>
        /// <param name="action">回调</param>
        /// <param name="duration">触发所需的时间</param>
        /// <param name="loop">循环次数（设置负数为将会一直循环，例如 -1）</param>
        /// <returns>计时器 ID</returns>
        public uint FixedTiming(Action<float> action, float duration, int loop)
        {
            timerIncrementId++;

            TimerInfo info = new()
            {
                id = timerIncrementId,
                action = action,
                duration = duration,
                loop = loop
            };
            fixedTimerDict.Add(info.id, info);

            return info.id;
        }

        private List<TimerInfo> infoTemps = new();
        private List<uint> timerTemps = new();
        private void TickTimerInfos(TickType tt, float tick)
        {
            var infos = TickType.Tick == tt ? timerDict : fixedTimerDict;
            var recyList = TickType.Tick == tt ? recyTimers : recyFixedTimers;
            Action<uint> stopTimerAction = TickType.Tick == tt ?  StopTimer : StopFixedTimer;
            while (recyList.TryDequeue(out var tid)) infos.Remove(tid);
            
            if (0 == infos.Count) return;
            infoTemps.Clear();
            timerTemps.Clear();
            foreach (var kv in infos)
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
                infos.Remove(infoTemp.id);
                infos.Add(infoTemp.id, infoTemp);
            }

            foreach (var timer in timerTemps)
            {
                if (infos.TryGetValue(timer, out var action) && false == recyList.Contains(action.id))
                {
                    action.action.Invoke(tick);
                    if (0 == action.loop) stopTimerAction(action.id);
                }
            }
        }
        #endregion
    }
}
