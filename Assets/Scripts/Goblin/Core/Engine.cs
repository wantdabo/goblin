using Goblin.Common;
using Goblin.Common.FSM;
using Goblin.Common.GameRes;
using Goblin.Common.Network;
using Goblin.Phases.Common;
using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.Core
{
    /// <summary>
    /// 游戏引擎组件
    /// </summary>
    public class Engine : Comp
    {
        /// <summary>
        /// 事件
        /// </summary>
        public Eventor eventor { get; set; }
        /// <summary>
        /// 对象池
        /// </summary>
        public ObjectPool pool { get; set; }
        /// <summary>
        /// 随机器
        /// </summary>
        public Common.Random random { get; set; }
        /// <summary>
        /// 引擎 Tick
        /// </summary>
        public Ticker ticker { get; set; }
        /// <summary>
        /// 游戏资源
        /// </summary>
        public GameRes gameres { get; set; }
        /// <summary>
        /// U3D API
        /// </summary>
        public U3DKit u3dkit { get; set; }
        /// <summary>
        /// 配置表
        /// </summary>
        public Config cfg { get; set; }
        /// <summary>
        /// 网络
        /// </summary>
        public NetNode net { get; set; }
        /// <summary>
        /// Proxy
        /// </summary>
        public GameProxy proxy { get; set; }
        /// <summary>
        /// 游戏 UI
        /// </summary>
        public GameUI gameui { get; set; }
        /// <summary>
        /// 游戏阶段
        /// </summary>
        public Phase phase { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();

            pool = AddComp<ObjectPool>();
            pool.Create();

            random = AddComp<Common.Random>();
            random.Initial(int.MaxValue);
            random.Create();

            ticker = AddComp<Ticker>();
            ticker.Create();

            gameres = AddComp<GameRes>();
            gameres.Create();

            u3dkit = AddComp<U3DKit>();
            u3dkit.Create();

            cfg = AddComp<Config>();
            cfg.Create();

            net = AddComp<NetNode>();
            net.Create();

            proxy = AddComp<GameProxy>();
            proxy.Create();

            gameui = AddComp<GameUI>();
            gameui.Create();

            gameui.Open<Sys.Gameplay.View.GameplayView>();
            // phase = AddComp<Phase>();
            // phase.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// 创建一个游戏引擎
        /// </summary>
        /// <returns>游戏引擎组件</returns>   
        public static Engine CreateEngine()
        {
            Engine engine = new();
            engine.engine = engine;
            engine.parent = engine;
            engine.Create();

            return engine;
        }
    }
}
