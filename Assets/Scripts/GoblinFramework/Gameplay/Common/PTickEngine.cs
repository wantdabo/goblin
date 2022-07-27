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
    public interface IPLoop { public void PLoop(int frame); }

    /// <summary>
    /// Play-Tick-Engine, 玩法 Tick 驱动组件
    /// </summary>
    public class PTickEngine : Comp<PGEngine>
    {
        private List<IPLoop> iploops = new List<IPLoop>();

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            iploops.Clear();
            iploops = null;

            base.OnDestroy();
        }

        public void AddPLoop(IPLoop update) { iploops.Add(update); }
        public void RmvPLoop(IPLoop update) { iploops.Remove(update); }

        public void PLoop(int frame)
        {
            if (0 > iploops.Count) return;
            for (int i = iploops.Count - 1; i >= 0; i--) iploops[i].PLoop(frame);
        }
    }
}
