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
        public TickEngine TickEngine = null;
        public U3DTool U3D = null;
        public GameUI GameUI = null;
        public GameRes GameRes = null;

        protected override void OnCreate()
        {
            base.OnCreate();

            // 开发引擎（Unity3D）Tick
            TickEngine = AddComp<TickEngine>();
            // U3D API
            U3D = AddComp<U3DTool>();
            // 游戏 UI
            GameUI = AddComp<GameUI>();
            // 游戏资源
            GameRes = Engine.AddComp<YooGameRes>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            TickEngine = null;
            U3D = null;
            GameUI = null;
            GameRes = null;
        }
    }
}
