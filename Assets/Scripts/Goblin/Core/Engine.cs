using Goblin.Common;
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
        public Eventor eventor;
        public ObjectPool pool;
        public Common.Random random;
        public Ticker ticker;
        public GameRes gameRes;
        public U3DKit u3dkit;
        public Config cfg;
        public GameProxy proxy;
        public GameUI gameui;

        protected async override void OnCreate()
        {
            base.OnCreate();           

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
            gameRes = AddComp<YooGameRes>();
            await gameRes.Initial();
            gameRes.Create();

            // U3D API
            u3dkit = AddComp<U3DKit>();
            u3dkit.Create();

            // 配置表
            cfg = AddComp<Config>();
            cfg.Create();

            // Proxy
            proxy = AddComp<GameProxy>();
            proxy.Create();

            // 游戏 UI
            gameui = AddComp<GameUI>();
            gameui.Create();
            gameui.Open<Sys.GameInitialize.GameInitializeView>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            eventor = null;
            pool = null;
            random = null;
            ticker = null;
            u3dkit = null;
            gameui = null;
            gameRes = null;
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
