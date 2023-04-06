using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Common
{
    /// <summary>
    /// ILoop，战斗循环
    /// </summary>
    public interface ILoop
    {
        public void PLoop(int frame, float tick);
    }

    /// <summary>
    /// ILateLoop，战斗循环，延后
    /// </summary>
    public interface ILateLoop
    {
        public void PLateLoop(int frame, float tick);
    }

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

        private List<ILoop> loops = null;
        private List<ILateLoop> lateLoops = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            loops = new List<ILoop>();
            lateLoops = new List<ILateLoop>();
        }

        protected override void OnDestroy()
        {
            loops.Clear();
            loops = null;

            lateLoops.Clear();
            lateLoops = null;

            base.OnDestroy();
        }

        public void AddLoop(ILoop loop)
        {
            loops.Add(loop);
        }

        public void RmvLoop(ILoop loop)
        {
            loops.Remove(loop);
        }

        public void AddLateLoop(ILateLoop lateLoop)
        {
            lateLoops.Add(lateLoop);
        }

        public void RmvLateLoop(ILateLoop lateLoop)
        {
            lateLoops.Remove(lateLoop);
        }

        public void PLoop(float t)
        {
            tick = t;
            frame += 1;
            
            if (loops.Count > 0)
                for (int i = loops.Count - 1; i >= 0; i--)
                    loops[i].PLoop(frame, t);
            
            if (lateLoops.Count > 0)
                for (int i = lateLoops.Count - 1; i >= 0; i--)
                    lateLoops[i].PLateLoop(frame, t);
        }
    }
}