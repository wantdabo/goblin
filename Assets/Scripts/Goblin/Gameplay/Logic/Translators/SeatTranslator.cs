using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 座位信息翻译器
    /// </summary>
    public class SeatTranslator : Translator<SeatInfo>
    {
        protected override void OnRIL(SeatInfo info)
        {
            foreach (var kv in info.asdict)
            {
                stage.rilsync.Send(RIL_DEFINE.TYPE_RENDER, kv.Key, new RIL_SEAT
                {
                    seat = kv.Value 
                });
            }
        }
    }
}