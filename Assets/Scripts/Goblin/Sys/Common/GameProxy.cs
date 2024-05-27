using Goblin.Common;
using Goblin.Core;
using Goblin.Sys.Lobby;
using Goblin.Sys.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Sys.Common
{
    /// <summary>
    /// 系统层，代理
    /// </summary>
    public class Proxy : Comp
    {
        public Eventor eventor;

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            eventor = null;
        }
    }

    /// <summary>
    /// 数据结构
    /// </summary>
    public abstract class Module : Comp
    {
        /// <summary>
        /// Proxy
        /// </summary>
        public Proxy proxy;
    }

    /// <summary>
    /// 数据结构
    /// </summary>
    /// <typeparam name="T">Proxy 类型</typeparam>
    public class Module<T> : Module where T : Proxy
    {
        /// <summary>
        /// Proxy
        /// </summary>
        public new T proxy
        {
            get { return base.proxy as T; }
        }
    }

    /// <summary>
    /// 数据缓存的 Proxy
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class Proxy<T> : Proxy where T : Module, new()
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; protected set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            data = AddComp<T>();
            data.proxy = this;
            data.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }

    /// <summary>
    /// Proxy 管理
    /// </summary>
    public class GameProxy : Comp
    {
        /// <summary>
        /// Proxy 集合
        /// </summary>
        private Dictionary<Type, Proxy> proxyDict = new();

        public LoginProxy login => GetProxy<LoginProxy>();
        public LobbyProxy lobby => GetProxy<LobbyProxy>();

        protected override void OnCreate()
        {
            base.OnCreate();
            Register<LoginProxy>();
            Register<LobbyProxy>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public T GetProxy<T>() where T : Proxy
        {
            if (proxyDict.TryGetValue(typeof(T), out var proxy)) return proxy as T;

            return null;
        }

        private void UnRegister<T>() where T : Proxy
        {
            if (false == proxyDict.TryGetValue(typeof(T), out var p)) return;

            RmvComp(p);
            p.Destroy();
        }

        private void Register<T>() where T : Proxy, new()
        {
            UnRegister<T>();

            var proxy = AddComp<T>();
            proxyDict.Add(typeof(T), proxy);
            proxy.Create();
        }
    }
}
