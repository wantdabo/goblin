using Goblin.Gameplay.Logic.Translation;
using Goblin.Gameplay.Logic.Translation.Common;

namespace Goblin.Gameplay.Logic.Common.StateMachine
{
    public class Translator : Translator<ParallelMachine>
    {
        public uint layerszero { get; set; }
        public uint layersone { get; set; }

        public override void Create()
        {
            base.Create();
            layerszero = State.NULL;
            layersone = State.NULL;
        }

        protected override void OnRIL()
        {
            uint zframes = 0;
            var machinez = behavior.GetMachine(ParallelMachine.LAYER_ZERO);
            var zero = State.NULL;
            if (null != machinez)
            {
                zero = machinez.current.id;
                zframes = machinez.current.frames;
            }
            if (zero != layerszero)
            {
                layerszero = zero;
                behavior.actor.stage.rilsync.SetRIL(behavior.actor.id, new RIL_STATEMACHINE() { sid = layerszero, frames = zframes, layer = ParallelMachine.LAYER_ZERO });
            }

            uint oframes = 0;
            var machineo = behavior.GetMachine(ParallelMachine.LAYER_ONE);
            var one = State.NULL;
            if (null != machineo)
            {
                one = machineo.current.id;
                oframes = machineo.current.frames;
            }
            if (one != layersone)
            {
                layersone = one;
                behavior.actor.stage.rilsync.SetRIL(behavior.actor.id, new RIL_STATEMACHINE() { sid = layersone, frames = oframes, layer = ParallelMachine.LAYER_ONE });
            }
        }
    }
}
