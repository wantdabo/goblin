using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Logic.Common.StateMachine
{
    /// <summary>
    /// 并发状态机翻译
    /// </summary>
    public class Translator : Translator<ParallelMachine>
    {
        /// <summary>
        /// 零层状态
        /// </summary>
        public uint lzerostate { get; set; }
        /// <summary>
        /// 一层状态
        /// </summary>
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
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_STATE_MACHINE_ZERO(zerostate, lzerostate, StateDef.LAYER_ZERO));
                lzerostate = zerostate;
            }

            var machineo = behavior.GetMachine(StateDef.LAYER_ONE);
            var onestate = StateDef.NULL;
            if (null != machineo && null != machineo.current)
            {
                onestate = machineo.current.id;
            }

            if (onestate != lonestate)
            {
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_STATE_MACHINE_ONE(onestate, lonestate, StateDef.LAYER_ONE));
                lonestate = onestate;
            }
        }
    }
}
