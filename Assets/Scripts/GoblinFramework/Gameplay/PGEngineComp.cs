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
    /// Play-Game-Engine-Comp 战斗的引擎组件
    /// </summary>
    public class PGEngineComp : GameEngineComp<PGEngineComp>
    {
        public PTickEngineComp PTickEngine = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            PTickEngine = AddComp<PTickEngineComp>();
        }
    }
}
