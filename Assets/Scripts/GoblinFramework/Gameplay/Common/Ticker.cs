using GoblinFramework.Core;
using GoblinFramework.Gameplay.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Common
{
    /// <summary>
    /// Ticker, 逻辑层 Tick 驱动组件
    /// </summary>
    public class Ticker : Actor
    {
        private int mFrame = 0;

        public int frame
        {
            get { return mFrame; }
            private set { mFrame = value; }
        }

        private float mTick = 1 / 60f;

        public float tick
        {
            get { return mTick; }
            private set { mTick = value; }
        }

        public void Tick(float t)
        {
            tick = t;
            frame += 1;

            var tickEvent = new TickEvent();
            tickEvent.frame = frame;
            tickEvent.tick = tick;
            eventor.Tell(tickEvent);

            var lateTickEvent = new LateTickEvent();
            tickEvent.frame = frame;
            tickEvent.tick = tick;
            eventor.Tell(lateTickEvent);
        }
    }
}