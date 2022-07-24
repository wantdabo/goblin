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
    /// IGameplayLoop，战斗循环
    /// </summary>
    public interface IGLoop { public void Update(float tick); }

    /// <summary>
    /// Play-Tick-Engine-Comp, 玩法 Tick 驱动组件
    /// </summary>
    public class PTickEngineComp : Comp<PGEngineComp>
    {
        private List<IGLoop> igLoops = new List<IGLoop>();

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            igLoops.Clear();
            igLoops = null;

            base.OnDestroy();
        }

        public void AddUpdate(IGLoop update) { igLoops.Add(update); }
        public void RmvUpdate(IGLoop update) { igLoops.Remove(update); }

        public void Update(float tick)
        {
            if (0 > igLoops.Count) return;
            for (int i = igLoops.Count - 1; i >= 0; i--) igLoops[i].Update(tick);
        }
    }
}
