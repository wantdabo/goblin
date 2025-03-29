using Goblin.Common;
using Goblin.Core;

namespace Goblin.Gameplay.Render.Core
{
    public sealed class World : Comp
    {
        /// <summary>
        /// 事件订阅派发者
        /// </summary>
        public Eventor eventor { get; private set; }
        /// <summary>
        /// Ticker/时间驱动器
        /// </summary>
        public Ticker ticker { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
            
            ticker = AddComp<Ticker>();
            ticker.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}