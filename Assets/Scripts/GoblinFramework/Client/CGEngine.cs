using GoblinFramework.Client.Common;
using GoblinFramework.Client.GameResource;
using GoblinFramework.Client.UI;
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
    public class CGEngine : GameEngine<CGEngine>
    {
        public Ticker Ticker = null;
        public U3DTool U3D = null;
        public GameUI GameUI = null;
        public GameRes GameRes = null;

        protected async override void OnCreate()
        {
            base.OnCreate();

            // 引擎 Tick
            Ticker = AddComp<Ticker>();
            Ticker.Create();

            // 游戏资源
            GameRes = AddComp<YooGameRes>();
            GameRes.Create();
            await GameRes.InitialGameRes();

            // U3D API
            U3D = AddComp<U3DTool>();
            U3D.Create();

            // 游戏 UI
            GameUI = AddComp<GameUI>();
            GameUI.Create();

            Engine.GameUI.OpenUI<UI.GameInitialize.GameInitializeView>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Ticker = null;
            U3D = null;
            GameUI = null;
            GameRes = null;
        }
    }
}
