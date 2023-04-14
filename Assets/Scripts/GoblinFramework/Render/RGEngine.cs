using GoblinFramework.Render.Common;
using GoblinFramework.Render.GameResource;
using GoblinFramework.Render.UI;
using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Render
{
    /// <summary>
    /// Render-Game-Engine-Comp 客户端引擎组件
    /// </summary>
    public class RGEngine : GameEngine<RGEngine>
    {
        public Ticker ticker = null;
        public U3DTool u3dtool = null;
        public GameUI gameui = null;
        public GameRes gameRes = null;

        protected async override void OnCreate()
        {
            base.OnCreate();

            // 引擎 Tick
            ticker = AddComp<Ticker>();
            ticker.Create();

            // 游戏资源
            gameRes = AddComp<YooGameRes>();
            gameRes.Create();
            await gameRes.InitialGameRes();

            // U3D API
            u3dtool = AddComp<U3DTool>();
            u3dtool.Create();

            // 游戏 UI
            gameui = AddComp<GameUI>();
            gameui.Create();

            engine.gameui.Open<UI.GameInitialize.GameInitializeView>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ticker = null;
            u3dtool = null;
            gameui = null;
            gameRes = null;
        }
    }
}
