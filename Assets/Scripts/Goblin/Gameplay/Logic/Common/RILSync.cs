using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Translations.Common;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Common
{
    /// <summary>
    /// RIL/ 渲染指令同步
    /// </summary>
    public class RILSync : Comp
    {
        /// <summary>
        /// 场景
        /// </summary>
        public Stage stage { get; set; }
        /// <summary>
        /// 指令集合
        /// </summary>
        private Dictionary<uint, Dictionary<ushort, IRIL>> rildict = new();

        /// <summary>
        /// 推送渲染指令
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="ril">渲染指令</param>
        public void PushRIL(uint id, IRIL ril)
        {
            if (false == rildict.TryGetValue(id, out var dict)) rildict[id] = dict = new Dictionary<ushort, IRIL>();
            if (dict.TryGetValue(ril.id, out var oldril))
            {
                if (oldril.Equals(ril)) return;

                dict.Remove(ril.id);
            }
            dict.Add(ril.id, ril);

            // 状态变化，通知
            stage.eventor.Tell(new RILSyncEvent { id = id, frame = stage.ticker.frame, ril = ril });
        }
    }
}
