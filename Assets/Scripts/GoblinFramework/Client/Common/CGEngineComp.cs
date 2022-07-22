using GoblinFramework.Client.Common;
using GoblinFramework.Client.GameRes;
using GoblinFramework.Client.GameStages;
using GoblinFramework.Client.UI.Common;
using GoblinFramework.General;
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
        public TickEngineComp TickEngine = null;
        public U3DComp U3D = null;
        public GameUIComp GameUI = null;
        public GameResComp GameRes = null;
        public GameStageComp GameStage = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            // 开发引擎（Unity3D）Tick
            TickEngine = AddComp<TickEngineComp>();
            // U3D 开发，辅助 API
            U3D = AddComp<U3DComp>();
            // 游戏 UI
            GameUI = AddComp<GameUIComp>();
            // 游戏美术资源
            GameRes = Engine.AddComp<YooGameResComp>();
            // 游戏阶段总控
            GameStage = AddComp<GameStageComp>();

            // 进入资源检查阶段
            GameStage.EnterState<GameInitializeState>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TickEngine = null;
            U3D = null;
            GameUI = null;
            GameRes = null;
            GameStage = null;
        }
    }
}
