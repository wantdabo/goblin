using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Render.Core
{
    public struct RILResolveEvent : IEvent
    {
        public uint frame { get; set; }
        public IRIL ril { get; set; }
    }

    public abstract class Resolver : Comp
    {
        public abstract ushort id { get; }
        public Actor actor { get; set; }

        public void Awake(uint frame, IRIL ril)
        {
            OnAwake(frame, ril);
        }

        public void Resolve(uint frame, IRIL ril)
        {
            OnResolve(frame, ril);
        }

        protected abstract void OnAwake(uint frame, IRIL ril);
        protected abstract void OnResolve(uint frame, IRIL ril);
    }

    public abstract class Resolver<TR> : Resolver where TR : IRIL
    {
        protected override void OnAwake(uint frame, IRIL ril)
        {
            OnAwake(frame, (TR)ril);
        }

        protected override void OnResolve(uint frame, IRIL ril)
        {
            OnResolve(frame, (TR)ril);
        }

        protected abstract void OnAwake(uint frame, TR ril);
        protected abstract void OnResolve(uint frame, TR ril);
    }
}
