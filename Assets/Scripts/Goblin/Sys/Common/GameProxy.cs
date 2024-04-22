using Goblin.Common;
using Goblin.Core;
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
    public class Module<T> : Comp where T : Proxy
    {
    }

    /// <summary>
    /// 数据缓存的 Proxy
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class Proxy<T> : Proxy where T : Module<Proxy<T>>, new()
    {

    }

    /// <summary>
    /// Proxy 管理
    /// </summary>
    public class GameProxy: Comp
    {
        private Dictionary<Type, Proxy> proxyDict = new();

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public T GetProxy<T>() where T : Proxy
        {
            if(proxyDict.TryGetValue(typeof(T), out var proxy))
                return proxy as T;

            return null;
        }

        private void UnRegister<T>() where T : Proxy
        {
            if(false == proxyDict.TryGetValue(typeof(T), out var p))
                return;
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
