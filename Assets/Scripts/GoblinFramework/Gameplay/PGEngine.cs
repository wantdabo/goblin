using GoblinFramework.Core;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay
{
    /// <summary>
    /// Play-Game-Engine 战斗的引擎组件
    /// </summary>
    public class PGEngine : GameEngine<PGEngine>
    {
        public TickEngine TickEngine = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            TickEngine = AddComp<TickEngine>();
        }
    }
}
