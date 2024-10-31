using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Buffs.Common
{
    /// <summary>
    /// Buff 桶翻译
    /// </summary>
    public class Translator : Translator<BuffBucket>
    {
        /// <summary>
        /// Buff 信息字典
        /// </summary>
        private Dictionary<uint, (byte state, uint layer, uint maxlayer)> buffdict { get; set; } = new();

        protected override void OnRIL()
        {
            foreach (uint id in behavior.buffs)
            {
                var buff = behavior.Get(id);
                if (null == buff) continue;

                if (false == buffdict.TryGetValue(id, out var info))
                {
                    info = (BUFF_STATE_DEFINE.INACTIVE, 0, 0);
                    buffdict.Add(id, info);
                }

                if (buff.state != info.state || buff.layer != info.layer || buff.maxlayer != info.maxlayer)
                {
                    buffdict.Remove(id);
                    buffdict.Add(id, (buff.state, buff.layer, buff.maxlayer));
                    behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_BUFF_INFO(buff.id, buff.state, buff.layer, buff.maxlayer));
                }
            }
        }
    }
}
