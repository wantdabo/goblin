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
            lzerostate = STATE_DEFINE.NULL;
            lonestate = STATE_DEFINE.NULL;
        }

        protected override void OnRIL()
        {
            var machinez = behavior.GetMachine(STATE_DEFINE.LAYER_ZERO);
            var zerostate = STATE_DEFINE.NULL;
            if (null != machinez && null != machinez.current)
            {
                zerostate = machinez.current.id;
            }

            if (zerostate != lzerostate)
            {
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_STATE_MACHINE_ZERO(zerostate, lzerostate, STATE_DEFINE.LAYER_ZERO));
                lzerostate = zerostate;
            }

            var machineo = behavior.GetMachine(STATE_DEFINE.LAYER_ONE);
            var onestate = STATE_DEFINE.NULL;
            if (null != machineo && null != machineo.current)
            {
                onestate = machineo.current.id;
            }

            if (onestate != lonestate)
            {
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_STATE_MACHINE_ONE(onestate, lonestate, STATE_DEFINE.LAYER_ONE));
                lonestate = onestate;
            }
        }
    }
}
