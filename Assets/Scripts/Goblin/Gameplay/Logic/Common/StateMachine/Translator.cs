using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Logic.Common.StateMachine
{
    public class Translator : Translator<ParallelMachine>
    {
        public uint lzerostate { get; set; }
        public uint lzeroframes { get; set; }
        public uint lonestate { get; set; }
        public uint loneframes { get; set; }

        public override void Create()
        {
            base.Create();
            lzerostate = StateDef.NULL;
            lonestate = StateDef.NULL;
        }

        protected override void OnRIL()
        {
            uint zeroframes = lzeroframes;
            var machinez = behavior.GetMachine(StateDef.LAYER_ZERO);
            var zerostate = StateDef.NULL;
            if (null != machinez && null != machinez.current)
            {
                zerostate = machinez.current.id;
                zeroframes = machinez.current.frames;
            }
            if (zeroframes != lzeroframes)
            {
                lzeroframes = zeroframes;
                if (zerostate != lzerostate)
                {
                    lzerostate = zerostate;
                    lzeroframes = 0;
                }
                lzerostate = zerostate;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_STATEMACHINE_ZERO(lzerostate, lzeroframes, StateDef.LAYER_ZERO));
            }

            uint oneframes = loneframes;
            var machineo = behavior.GetMachine(StateDef.LAYER_ONE);
            var onestate = StateDef.NULL;
            if (null != machineo && null != machineo.current)
            {
                onestate = machineo.current.id;
                oneframes = machineo.current.frames;
            }
            if (oneframes != loneframes)
            {
                loneframes = oneframes;
                if (onestate != lonestate)
                {
                    lonestate = onestate;
                    loneframes = 0;
                }

                lonestate = onestate;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_STATEMACHINE_ONE(lonestate, loneframes, StateDef.LAYER_ONE));
            }
        }
    }
}
