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
        public U3DTool u3d = null;
        public GameUI ui = null;
        public GameRes res = null;

        protected async override void OnCreate()
        {
            base.OnCreate();

            // 引擎 Tick
            ticker = AddComp<Ticker>();
            ticker.Create();

            // 游戏资源
            res = AddComp<YooGameRes>();
            res.Create();
            await res.InitialGameRes();

            // U3D API
            u3d = AddComp<U3DTool>();
            u3d.Create();

            // 游戏 UI
            ui = AddComp<GameUI>();
            ui.Create();

            engine.ui.Open<UI.GameInitialize.GameInitializeView>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ticker = null;
            u3d = null;
            ui = null;
            res = null;
        }
    }
}
