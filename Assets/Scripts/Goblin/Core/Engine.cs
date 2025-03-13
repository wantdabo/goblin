using Goblin.Common;
using Goblin.Common.FSM;
using Goblin.Common.GameRes;
using Goblin.Common.Network;
using Goblin.Common.Parallel;
using Goblin.Common.Sounds;
using Goblin.Phases.Common;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
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
        public Eventor eventor { get; private set; }
        /// <summary>
        /// 对象池
        /// </summary>
        public ObjectPool pool { get; private set; }
        /// <summary>
        /// 随机器
        /// </summary>
        public Common.Random random { get; private set; }
        /// <summary>
        /// 引擎 Tick
        /// </summary>
        public Ticker ticker { get; private set; }
        /// <summary>
        /// 协程驱动器
        /// </summary>
        public CoroutineScheduler scheduler { get; private set; }
        /// <summary>
        /// 游戏资源
        /// </summary>
        public GameRes gameres { get; private set; }
        /// <summary>
        /// U3D API
        /// </summary>
        public U3DKit u3dkit { get; private set; }
        /// <summary>
        /// 配置表
        /// </summary>
        public Config cfg { get; private set; }
        /// <summary>
        /// 网络
        /// </summary>
        public NetNode net { get; private set; }
        /// <summary>
        /// 音效
        /// </summary>
        public Sound sound { get; private set; }
        /// <summary>
        /// Proxy
        /// </summary>
        public GameProxy proxy { get; private set; }
        /// <summary>
        /// 游戏 UI
        /// </summary>
        public GameUI gameui { get; private set; }
        /// <summary>
        /// 游戏阶段
        /// </summary>
        public Phase phase { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();

            pool = AddComp<ObjectPool>();
            pool.Create();

            random = AddComp<Common.Random>();
            random.Initial(19491001);
            random.Create();

            ticker = AddComp<Ticker>();
            ticker.Create();

            scheduler = AddComp<CoroutineScheduler>();
            scheduler.Initialize(TickType.Tick, ticker);

            gameres = AddComp<GameRes>();
            gameres.Create();

            u3dkit = AddComp<U3DKit>();
            u3dkit.Create();

            cfg = AddComp<Config>();
            cfg.Create();
#if UNITY_WEBGL
            net = AddComp<NetWebSocket>();
#else
            net = AddComp<NetTCP>();
#endif
            net.Create();

            sound = AddComp<Sound>();
            sound.Create();

            proxy = AddComp<GameProxy>();
            proxy.Create();

            gameui = AddComp<GameUI>();
            gameui.Create();

            // gameui.Open<Sys.Other.View.FrameworkView>();
            phase = AddComp<Phase>();
            phase.Create();
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
