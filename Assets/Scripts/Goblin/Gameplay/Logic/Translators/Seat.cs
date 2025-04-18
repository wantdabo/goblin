using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 座位信息翻译器
    /// </summary>
    public class Seat : Translator<SeatInfo>
    {
        protected override void OnRIL(SeatInfo info)
        {
            foreach (var kv in info.asdict)
            {
                stage.rilsync.Push(kv.Key, new RIL_SEAT { seat = kv.Value });
            }
        }
    }
}