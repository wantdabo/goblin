using GoblinFramework.Core;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Theaters;
using GoblinFramework.General;
using GoblinFramework.General.Gameplay.Command.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay
{
    /// <summary>
    /// Play-Game-Engine 战斗的引擎组件
    /// </summary>
    public class PGEngine : GameEngine<PGEngine>
    {
        public TickEngine TickEngine = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            TickEngine = AddComp<TickEngine>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TickEngine = null;
        }

        /// <summary>
        /// TODO 临时代码，测试管道，记得删除哟
        /// </summary>
        private Theater Theater = null;
        public void BeginGame()
        {
            Theater = AddComp<Theater>();
        }

        /// <summary>
        /// TODO 临时代码，测试管道，记得删除哟
        /// </summary>
        private Client.Gameplay.Theater CTheater;
        public void RegisterClientTheater(Client.Gameplay.Theater ctheater)
        {
            CTheater = ctheater;
        }

        /// <summary>
        /// TODO 临时代码，测试管道，记得删除哟
        /// </summary>
        public void ToSyncCMD<T>(T cmd) where T : SyncCmd
        {
            CTheater.Resolve(cmd);
        }
    }
}
