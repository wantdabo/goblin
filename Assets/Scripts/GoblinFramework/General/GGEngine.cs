using GoblinFramework.Core;
using GoblinFramework.General.Gameplay;
using GoblinFramework.General.Gameplay.RIL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General
{
    /// <summary>
    /// General-Game-Engine 通用的引擎组件
    /// </summary>
    public class GGEngine : GameEngine<GGEngine>
    {
        public RILParser RILParser;
        public Config Config;

        protected override void OnCreate()
        {
            base.OnCreate();

            // 指令翻译
            RILParser = AddComp<RILParser>();
            // 游戏配置
            Config = AddComp<Config>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
