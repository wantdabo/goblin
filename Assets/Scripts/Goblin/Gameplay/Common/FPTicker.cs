using Goblin.Common;
using Goblin.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Goblin.Gameplay.Common
{
    /// <summary>
    /// Tick 事件
    /// </summary>
    public struct FPTickEvent : IEvent
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame;
        /// <summary>
        /// 流逝的时间 s/秒
        /// </summary>
        public FP tick;
    }

    /// <summary>
    /// LateTick 事件
    /// </summary>
    public struct FPLateTickEvent : IEvent
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame;
        /// <summary>
        /// 流逝的时间 s/秒
        /// </summary>
        public FP tick;
    }

    /// <summary>
    /// 确定性 Ticker, 逻辑层 Tick 驱动组件
    /// </summary>
    public class FPTicker : Comp
    {
        /// <summary>
        /// 事件订阅/派发者
        /// </summary>
        public Eventor eventor;

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
        public FP tick { get; private set; } = FP.One / (2 * FP.Ten);

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

        /// <summary>
        /// 流逝时间
        /// </summary>
        /// <param name="t">s/秒</param>
        public void Tick()
        {
            frame++;
            // 驱动计时器
            TickTimerInfos(timerInfos, tick);
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
            public uint id;
            /// <summary>
            /// Callback/回调
            /// </summary>
            public Action<FP> action;
            /// <summary>
            /// 触发所需的时间
            /// </summary>
            public FP duration;
            /// <summary>
            /// 当前过去了多少时间
            /// </summary>
            public FP elapsed;
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
            timerInfos.Add(info);

            return info.id;
        }

        private void TickTimerInfos(List<TimerInfo> infos, FP tick)
        {
            if (0 == infos.Count) return;

            for (int i = infos.Count - 1; i >= 0; i--)
            {
                var info = infos[i];
                info.elapsed += tick;
                infos[i] = info;
                if (info.duration > info.elapsed) continue;

                info.elapsed = TSMath.Max(0, info.elapsed - info.duration);
                info.loop--;
                infos[i] = info;
                if (0 == info.loop) infos.RemoveAt(i);

                info.action.Invoke(tick);
            }
        }
        #endregion
    }
}
