using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Translators;

namespace Goblin.Gameplay.Logic.Behaviors.Batchs
{
    /// <summary>
    /// 翻译 RIL 渲染指令, 所有 Stage BehaviorInfo 来生成渲染指令
    /// </summary>
    public class Translate : Behavior
    {
        /// <summary>
        /// RIL 翻译器集合
        /// </summary>
        public Dictionary<Type, Logic.Translators.Common.Translator> translators { get; set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();
            translators = ObjectCache.Get<Dictionary<Type, Translators.Common.Translator>>();
            translators.Add(typeof(StageInfo), ObjectCache.Get<Translators.StageTranslator>().Load(stage));
            translators.Add(typeof(TickerInfo), ObjectCache.Get<Translators.TickerTranslator>().Load(stage));
            translators.Add(typeof(SeatInfo), ObjectCache.Get<Translators.SeatTranslator>().Load(stage));
            translators.Add(typeof(TagInfo), ObjectCache.Get<Translators.TagTranslator>().Load(stage));
            translators.Add(typeof(AttributeInfo), ObjectCache.Get<Translators.AttributeTranslator>().Load(stage));
            translators.Add(typeof(SpatialInfo), ObjectCache.Get<Translators.SpatialTranslator>().Load(stage));
            translators.Add(typeof(StateMachineInfo),  ObjectCache.Get<Translators.StateMachineTranslator>().Load(stage));
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            foreach (var translator in translators.Values)
            {
                translator.Unload();
                ObjectCache.Set(translator);
            }
            translators.Clear();
            ObjectCache.Set(translators);
        }

        protected override void OnEndTick()
        {
            var info = stage.GetBehaviorInfo<StageInfo>(stage.sa);
            
            if (translators.TryGetValue(typeof(StageInfo), out var t)) t.Translate(info);
            foreach (var kv in info.behaviorinfos)
            {
                foreach (var behaviorinfo in kv.Value)
                {
                    if (false == translators.TryGetValue(kv.Key, out var translator)) continue;
                    translator.Translate(behaviorinfo);
                }
            }
        }
    }
}