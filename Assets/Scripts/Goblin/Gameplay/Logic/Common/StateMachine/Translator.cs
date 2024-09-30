using Goblin.Gameplay.Logic.Translation;
using Goblin.Gameplay.Logic.Translation.Common;

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
            lzerostate = State.NULL;
            lonestate = State.NULL;
        }

        protected override void OnRIL()
        {
            uint zeroframes = lzeroframes;
            var machinez = behavior.GetMachine(ParallelMachine.LAYER_ZERO);
            var zerostate = State.NULL;
            if (null != machinez)
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
                behavior.actor.stage.rilsync.SetRIL(behavior.actor.id, new RIL_STATEMACHINE_ZERO(lzerostate, lzeroframes, ParallelMachine.LAYER_ZERO));
            }

            uint oneframes = loneframes;
            var machineo = behavior.GetMachine(ParallelMachine.LAYER_ONE);
            var onestate = State.NULL;
            if (null != machineo)
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
                behavior.actor.stage.rilsync.SetRIL(behavior.actor.id, new RIL_STATEMACHINE_ONE(lonestate, loneframes, ParallelMachine.LAYER_ONE));
            }
        }
    }
}
