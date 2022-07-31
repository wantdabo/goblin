using GoblinFramework.Client.Common;
using GoblinFramework.Client.GameResource;
using GoblinFramework.Client.GameStages;
using GoblinFramework.Client.UI;
using GoblinFramework.General;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoblinFramework.Client.Gameplay;
using GoblinFramework.General.Gameplay.Command.Cmds;
using GoblinFramework.Gameplay;

namespace GoblinFramework.Client
{
    /// <summary>
    /// Client-Game-Engine-Comp 客户端引擎组件
    /// </summary>
    public class CGEngine : GameEngine<CGEngine>
    {
        public TickEngine TickEngine = null;
        public U3DTool U3D = null;
        public GameUI GameUI = null;
        public GameRes GameRes = null;
        public Theater Theater = null;
        public GameStage GameStage = null;

        protected override void OnCreate()
        {
            base.OnCreate();

            // 开发引擎（Unity3D）Tick
            TickEngine = AddComp<TickEngine>();
            // U3D API
            U3D = AddComp<U3DTool>();
            // 游戏 UI
            GameUI = AddComp<GameUI>();
            // 游戏美术资源
            GameRes = Engine.AddComp<YooGameRes>();
            // 游戏剧场
            Theater = AddComp<Theater>();
            // 游戏阶段总控
            GameStage = AddComp<GameStage>();
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
