using GoblinFramework.Core;
using GoblinFramework.Logic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Logic
{
    public class LGEngine : GameEngine<LGEngine>
    {
        public Ticker Ticker;

        protected override void OnCreate()
        {
            base.OnCreate();

            // 引擎 Tick
            Ticker = AddComp<Ticker>();
            Ticker.Create();
        }
    }
}
