using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Logic.Common.StateMachine
{
    public class Translator : Translator<ParallelMachine>
    {
        public uint lzerostate { get; set; }
        public uint lonestate { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            lzerostate = StateDef.NULL;
            lonestate = StateDef.NULL;
        }

        protected override void OnRIL()
        {
            var machinez = behavior.GetMachine(StateDef.LAYER_ZERO);
            var zerostate = StateDef.NULL;
            if (null != machinez && null != machinez.current)
            {
                zerostate = machinez.current.id;
            }

            if (zerostate != lzerostate)
            {
                lzerostate = zerostate;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_STATEMACHINE_ZERO(lzerostate, StateDef.LAYER_ZERO));
            }

            var machineo = behavior.GetMachine(StateDef.LAYER_ONE);
            var onestate = StateDef.NULL;
            if (null != machineo && null != machineo.current)
            {
                onestate = machineo.current.id;
            }

            if (onestate != lonestate)
            {
                lonestate = onestate;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_STATEMACHINE_ONE(lonestate, StateDef.LAYER_ONE));

            }
        }
    }
}
