using Goblin.Common;
using Goblin.Common.Network;
using Goblin.Common.Res;
using Goblin.Sys.Common;
using Queen.Network.Protocols;
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
        public Eventor eventor;
        public ObjectPool pool;
        public Common.Random random;
        public Ticker ticker;
        public GameRes gameres;
        public U3DKit u3dkit;
        public Config cfg;
        public NetNode net;
        public GameProxy proxy;
        public GameUI gameui;

        protected override void OnCreate()
        {
            base.OnCreate();
            ENet.Library.Initialize();

            // 事件
            eventor = AddComp<Eventor>();
            eventor.Create();

            // 对象池
            pool = AddComp<ObjectPool>();
            pool.Create();

            // 随机数
            random = AddComp<Common.Random>();
            random.Initial(int.MaxValue);
            random.Create();

            // 引擎 Tick
            ticker = AddComp<Ticker>();
            ticker.Create();

            // 游戏资源
            gameres = AddComp<GameRes>();
            gameres.Create();

            // U3D API
            u3dkit = AddComp<U3DKit>();
            u3dkit.Create();

            // 配置表
            cfg = AddComp<Config>();
            cfg.Create();

            // 网络
            net = AddComp<NetNode>();
            net.Create();

            // Proxy
            proxy = AddComp<GameProxy>();
            proxy.Create();

            // 游戏 UI
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
