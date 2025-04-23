using Goblin.Core;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    /// <summary>
    /// 代理赋能
    /// </summary>
    public abstract class AgentEnchant : Comp
    {
        protected StateBucket statebucket { get; private set; }
        
        public AgentEnchant Initialize(StateBucket statebucket)
        {
            this.statebucket = statebucket;

            return this;
        }
    }

    public abstract class AgentEnchant<T> : AgentEnchant where T : State
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            statebucket.eventor.Listen<RStateEvent<T>>(OnRState);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            statebucket.eventor.UnListen<RStateEvent<T>>(OnRState);
        }

        private void OnRState(RStateEvent<T> e)
        {
            OnRState(e.state);
        }
        
        protected abstract void OnRState(T state);
    }
}