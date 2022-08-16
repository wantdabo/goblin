using FixMath.NET;
using GoblinFramework.Core;
using GoblinFramework.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Common
{
    /// <summary>
    /// IPlayLoop，战斗循环
    /// </summary>
    public interface IPLoop { public void PLoop(int frame, Fix64 detailTime); }

    /// <summary>
    /// IPlayLateLoop，战斗循环，延后
    /// </summary>
    public interface IPLateLoop { public void PLateLoop(int frame, Fix64 detailTime); }

    /// <summary>
    /// Play-Tick-Engine, 玩法 Tick 驱动组件
    /// </summary>
    public class TickEngine : Comp<PGEngine>
    {
        private int mFrame = 0;
        public int frame { get { return mFrame; } private set { mFrame = value; } }

        public readonly Fix64 detailTime = 6666 * Fix64.EN5;

        private List<IPLoop> iploops = new List<IPLoop>();
        private List<IPLateLoop> iplateLoops = new List<IPLateLoop>();

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            iploops.Clear();
            iploops = null;

            iplateLoops.Clear();
            iplateLoops = null;

            base.OnDestroy();
        }

        public void AddPLoop(IPLoop loop) { iploops.Add(loop); }
        public void RmvPLoop(IPLoop update) { iploops.Remove(update); }
        public void AddPLateLoop(IPLateLoop lateLoop) { iplateLoops.Add(lateLoop); }
        public void RmvPLateLoop(IPLateLoop lateLoop) { iplateLoops.Remove(lateLoop); }

        public void PLoop()
        {
            frame += 1;
            if (0 == iploops.Count) return;
            for (int i = iploops.Count - 1; i >= 0; i--) iploops[i].PLoop(frame, detailTime);
            for (int i = iplateLoops.Count - 1; i >= 0; i--) iplateLoops[i].PLateLoop(frame, detailTime);
        }
    }
}
