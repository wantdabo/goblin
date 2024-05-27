using Goblin.Common;
using Goblin.Common.Network;
using Goblin.Common.Res;
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
        public Eventor eventor;
        /// <summary>
        /// 对象池
        /// </summary>
        public ObjectPool pool;
        /// <summary>
        /// 随机器
        /// </summary>
        public Common.Random random;
        /// <summary>
        /// 引擎 Tick
        /// </summary>
        public Ticker ticker;
        /// <summary>
        /// 游戏资源
        /// </summary>
        public GameRes gameres;
        /// <summary>
        /// U3D API
        /// </summary>
        public U3DKit u3dkit;
        /// <summary>
        /// 配置表
        /// </summary>
        public Config cfg;
        /// <summary>
        /// 网络
        /// </summary>
        public NetNode net;
        /// <summary>
        /// Proxy
        /// </summary>
        public GameProxy proxy;
        /// <summary>
        /// 游戏 UI
        /// </summary>
        public GameUI gameui;

        protected override void OnCreate()
        {
            base.OnCreate();
            ENet.Library.Initialize();

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

            gameui.Open<Sys.Other.View.FrameworkView>();
            gameui.Open<Sys.Login.View.LoginView>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ENet.Library.Deinitialize();
        }

        /// <summary>
        /// 创建一个游戏引擎
        /// </summary>
        /// <returns>游戏引擎组件</returns>
        public static Engine CreateEngine()
        {
            Engine engine = new();
            engine.engine = engine;
            engine.Create();

            return engine;
        }
    }
}
