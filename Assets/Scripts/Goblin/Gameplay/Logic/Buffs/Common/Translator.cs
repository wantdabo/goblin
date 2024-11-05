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
        private Dictionary<(uint, uint), (byte state, uint layer, uint maxlayer, uint from)> buffdict { get; set; } = new();

        protected override void OnRIL()
        {
            foreach (var buff in behavior.totalbuffs)
            {
                var key = (buff.id, buff.from);
                if (false == buffdict.TryGetValue(key, out var info))
                {
                    info = (BUFF_DEFINE.INACTIVE, 0, 0, 0);
                    buffdict.Add(key, info);
                }

                if (buff.state != info.state || buff.layer != info.layer || buff.maxlayer != info.maxlayer || buff.from != info.maxlayer)
                {
                    buffdict.Remove(key);
                    buffdict.Add(key, (buff.state, buff.layer, buff.maxlayer, buff.from));
                    behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_BUFF_INFO(buff.id, buff.type, buff.state, buff.layer, buff.maxlayer, buff.from));
                }
            }
        }
    }
}
