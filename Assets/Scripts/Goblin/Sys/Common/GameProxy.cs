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
