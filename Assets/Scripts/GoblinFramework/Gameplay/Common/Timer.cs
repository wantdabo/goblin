using GoblinFramework.Core;
using GoblinFramework.Gameplay.Events;
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;

namespace GoblinFramework.Gameplay.Common
{
    public enum TimerTickDef
    {
        Tick,
        LateTick
    }

    public class Timer : Comp
    {
        private struct TimeInfo
        {
            public uint id;
            public Action action;
            public uint duration;
            public uint elapsed;
            public int loop;
        }

        public Ticker ticker;
        private uint timerIncrementId = 0;
        private List<TimeInfo> timeInfos = new List<TimeInfo>();
        private List<TimeInfo> lateTimeInfos = new List<TimeInfo>();

        public void Create(Ticker t) 
        {
            ticker = t;
            Create();
        }

        public override void Create()
        {
            if (null == ticker) throw new Exception("plz use method Create(t : Ticker) to create timer.");
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            ticker.eventor.Listen<TickEvent>(OnTick);
            ticker.eventor.Listen<LateTickEvent>(OnLateTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ticker.eventor.UnListen<TickEvent>(OnTick);
            ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
            timeInfos.Clear();
            lateTimeInfos.Clear();
        }

        public void Stop(uint id, TimerTickDef tickDef = TimerTickDef.Tick)
        {
            Stop(id, TimerTickDef.Tick == tickDef ? timeInfos : lateTimeInfos);
        }

        private void Stop(uint id, List<TimeInfo> infos)
        {
            if (0 == infos.Count) return;

            for (int i = infos.Count; i >= 0; i--)
            {
                var info = infos[i];
                if (id != info.id) continue;

                infos.RemoveAt(i);
                break;
            }
        }

        public uint Timing(Action action, uint frame, int loop, TimerTickDef tickDef = TimerTickDef.Tick)
        {
            timerIncrementId++;

            TimeInfo info = new TimeInfo();
            info.id = timerIncrementId;
            info.action = action;
            info.duration = frame;
            info.loop = loop;

            if (TimerTickDef.Tick == tickDef) timeInfos.Add(info); else lateTimeInfos.Add(info);

            return info.id;
        }

        private void OnTick(TickEvent e)
        {
            TickTimeInfos(timeInfos, e.frame, e.tick);
        }

        private void OnLateTick(LateTickEvent e)
        {
            TickTimeInfos(lateTimeInfos, e.frame, e.tick);
        }

        private void TickTimeInfos(List<TimeInfo> infos, uint frame, float tick)
        {
            if (0 == infos.Count) return;

            for (int i = infos.Count - 1; i >= 0; i--)
            {
                var info = infos[i];
                info.elapsed++;

                if (info.duration > info.elapsed) continue;

                info.elapsed = 0;
                info.loop--;
                if (0 == info.loop) infos.RemoveAt(i);

                info.action.Invoke();
            }
        }
    }
}