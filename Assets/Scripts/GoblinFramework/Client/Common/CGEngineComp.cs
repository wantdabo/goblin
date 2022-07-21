using GoblinFramework.Client.Common;
using GoblinFramework.Client.GameRes;
using GoblinFramework.Client.GameStages;
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
        public GameStageComp GameStage = null;
        public TickEngineComp TickEngine = null;
        public U3DComp U3D = null;
        public GameUIComp GameUI = null;
        public GameResComp GameRes = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            TickEngine = AddComp<TickEngineComp>();
            U3D = AddComp<U3DComp>();
            GameUI = AddComp<GameUIComp>();
            GameRes = Engine.AddComp<YooGameResComp>();

            GameStage = AddComp<GameStageComp>();

            // 进入资源检查阶段
            GameStage.EnterState<GameStageGameResState>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameStage = null;
            TickEngine = null;
            U3D = null;
            GameUI = null;
            GameRes = null;
        }
    }
}
