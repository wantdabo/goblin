using GoblinFramework.Client;
using GoblinFramework.Client.Comps;
using GoblinFramework.Client.Comps.GameRes;
using GoblinFramework.Client.UI;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common
{
    public class GameEngineComp : Comp
    {
        public ClientTickComp EngineTick = null;
        public GameResComp GameRes = null;
        public GameUI GameUI = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            EngineTick = AddComp<ClientTickComp>();
            GameUI = AddComp<GameUI>();
            GameRes = AddComp<YooGameResComp>();
        }

        protected override void OnDestroy()
        {
            EngineTick = null;
            GameRes = null;
            base.OnDestroy();
        }
    }
}
