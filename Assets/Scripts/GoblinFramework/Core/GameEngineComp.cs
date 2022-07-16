using GoblinFramework.Client;
using GoblinFramework.Client.GameRes;
using GoblinFramework.Client.UI;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Core
{
    public class GameEngineComp : Comp
    {
        public CETickComp CETick = null;
        public GameResComp GameRes = null;
        public GameUI GameUI = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            GameRes = AddComp<YooGameResComp>();
            CETick = AddComp<CETickComp>();
            GameUI = AddComp<GameUI>();
        }

        protected override void OnDestroy()
        {
            CETick = null;
            GameRes = null;
            base.OnDestroy();
        }
    }
}
