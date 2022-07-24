using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General
{
    /// <summary>
    /// General-Game-Engine-Comp 通用的引擎组件
    /// </summary>
    public class GGEngineComp : GameEngineComp<GGEngineComp>
    {
        public ConfigComp Config;

        protected override void OnCreate()
        {
            base.OnCreate();

            // 游戏配置
            Config = AddComp<ConfigComp>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
