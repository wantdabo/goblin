using GoblinFramework.Client.Common;
using GoblinFramework.Client.GameRes;
using GoblinFramework.Client.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GoblinFramework.Core
{
    public class GameEngineComp : Comp
    {
        public CTEngineComp CTEngine = null;
        public GameResComp GameRes = null;
        public U3DComp U3D = null;
        public GameUI GameUI = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            CTEngine = AddComp<CTEngineComp>();
            GameUI = AddComp<GameUI>();
            U3D = AddComp<U3DComp>();
            GameRes = AddComp<YooGameResComp>();
        }

        protected override void OnDestroy()
        {
            CTEngine = null;
            GameRes = null;
            U3D = null;
            GameUI = null;
            base.OnDestroy();
        }
    }
}
