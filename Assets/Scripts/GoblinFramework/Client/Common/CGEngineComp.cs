using GoblinFramework.Client.Common;
using GoblinFramework.Client.GameRes;
using GoblinFramework.Client.UI.Common;
using GoblinFramework.Common;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Client
{
    /// <summary>
    /// Client-Game-Engine-Comp 客户端引擎组件
    /// </summary>
    public class CGEngineComp : GameEngineComp<CGEngineComp>
    {
        public CTEngineComp CTEngine = null;
        public U3DComp U3D = null;
        public GameResComp GameRes = null;
        public GameUI GameUI = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            CTEngine = AddComp<CTEngineComp>();
            U3D = AddComp<U3DComp>();
            GameUI = AddComp<GameUI>();
            GameRes = AddComp<YooGameResComp>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CTEngine = null;
            U3D = null;
            GameUI = null;
            GameRes = null;
        }
    }
}
