using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Common;

namespace Goblin.Gameplay.Render.Core
{
    public struct RILEvent : IEvent
    {
        public ABStateInfo state { get; set; }
    }

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
        public Summary rils { get; private set; }
        private Dictionary<ulong, List<Agent>> agents = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
            
            ticker = AddComp<Ticker>();
            ticker.Create();
            
            rils = AddComp<Summary>();
            rils.Initialize(this);
            rils.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}