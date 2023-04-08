using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Common
{
    /// <summary>
    /// Play-Tick, 逻辑层 Tick 驱动组件
    /// </summary>
    public class Ticker : Comp
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

        public event Action<float> onTick;
        public event Action<int, float> onLateTick;

        public void Tick(float t)
        {
            tick = t;
            frame += 1;

            onTick?.Invoke(t);
            onLateTick?.Invoke(frame, t);
        }
    }
}