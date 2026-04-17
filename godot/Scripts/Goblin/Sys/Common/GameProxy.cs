using Goblin.Common;
using Goblin.Core;
using System;
using System.Collections.Generic;

namespace Goblin.Sys.Common
{
    public class Proxy : Comp
    {
        public Eventor eventor { get; set; }

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

    public abstract class Model : Comp
    {
        public Proxy proxy;
    }

    public class Model<T> : Model where T : Proxy
    {
        public new T proxy => base.proxy as T;
    }

    public class Proxy<T> : Proxy where T : Model, new()
    {
        public T data { get; protected set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            data = AddComp<T>();
            data.proxy = this;
            data.Create();
        }
    }

    public class GameProxy : Comp
    {
        private Dictionary<Type, Proxy> proxyDict = new();

        public Goblin.Sys.Login.LoginProxy login => GetProxy<Goblin.Sys.Login.LoginProxy>();
        public Goblin.Sys.Lobby.LobbyProxy lobby => GetProxy<Goblin.Sys.Lobby.LobbyProxy>();
        public Goblin.Sys.Gameplay.GameplayProxy gameplay => GetProxy<Goblin.Sys.Gameplay.GameplayProxy>();

        protected override void OnCreate()
        {
            base.OnCreate();
            Register<Goblin.Sys.Login.LoginProxy>();
            Register<Goblin.Sys.Lobby.LobbyProxy>();
            Register<Goblin.Sys.Gameplay.GameplayProxy>();
        }

        public T GetProxy<T>() where T : Proxy
        {
            proxyDict.TryGetValue(typeof(T), out var proxy);
            return proxy as T;
        }

        private void Register<T>() where T : Proxy, new()
        {
            if (proxyDict.TryGetValue(typeof(T), out var old)) old.Destroy();
            var proxy = AddComp<T>();
            proxyDict[typeof(T)] = proxy;
            proxy.Create();
        }
    }
}
