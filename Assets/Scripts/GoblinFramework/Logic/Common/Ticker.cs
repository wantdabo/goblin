﻿using GoblinFramework.Core;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Logic.Common
{
    /// <summary>
    /// ILogicLoop，战斗循环
    /// </summary>
    public interface ILoop { public void PLoop(int frame, Fixed64 detailTime); }

    /// <summary>
    /// ILogicLateLoop，战斗循环，延后
    /// </summary>
    public interface ILateLoop { public void PLateLoop(int frame, Fixed64 detailTime); }

    /// <summary>
    /// Logic-Tick, 逻辑层 Tick 驱动组件
    /// </summary>
    public class Ticker : Comp<LGEngine>
    {
        private int mFrame = 0;
        public int frame { get { return mFrame; } private set { mFrame = value; } }

        public readonly Fixed64 detailTime = 625 * Fixed64.EN4;

        private List<ILoop> loops = new();
        private List<ILateLoop> lateLoops = new();

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            loops.Clear();
            loops = null;

            lateLoops.Clear();
            lateLoops = null;

            base.OnDestroy();
        }

        public void AddLoop(ILoop loop) { loops.Add(loop); }
        public void RmvLoop(ILoop loop) { loops.Remove(loop); }
        public void AddLateLoop(ILateLoop lateLoop) { lateLoops.Add(lateLoop); }
        public void RmvLateLoop(ILateLoop lateLoop) { lateLoops.Remove(lateLoop); }

        public void PLoop()
        {
            frame += 1;
            if (0 == loops.Count) return;
            for (int i = loops.Count - 1; i >= 0; i--) loops[i].PLoop(frame, detailTime);
            for (int i = lateLoops.Count - 1; i >= 0; i--) lateLoops[i].PLateLoop(frame, detailTime);
        }
    }
}