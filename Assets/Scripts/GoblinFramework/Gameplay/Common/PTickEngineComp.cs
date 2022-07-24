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
    public interface IPLoop { public void Update(float tick); }

    /// <summary>
    /// Play-Tick-Engine-Comp, 玩法 Tick 驱动组件
    /// </summary>
    public class PTickEngineComp : Comp<PGEngineComp>
    {
        private List<IPLoop> ipLoogs = new List<IPLoop>();

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            ipLoogs.Clear();
            ipLoogs = null;

            base.OnDestroy();
        }

        public void AddUpdate(IPLoop update) { ipLoogs.Add(update); }
        public void RmvUpdate(IPLoop update) { ipLoogs.Remove(update); }

        public void Update(float tick)
        {
            if (0 > ipLoogs.Count) return;
            for (int i = ipLoogs.Count - 1; i >= 0; i--) ipLoogs[i].Update(tick);
        }
    }
}
