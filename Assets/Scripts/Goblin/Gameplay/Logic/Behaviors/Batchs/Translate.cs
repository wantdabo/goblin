using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Translators;
using Goblin.Gameplay.Logic.Translators.Common;

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
        private Dictionary<Type, Translator> translators { get; set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();

            translators = ObjectCache.Get<Dictionary<Type, Translator>>();
            void Translator<T, E>() where T : Translator, new() where E : BehaviorInfo
            {
                var translator = ObjectCache.Get<T>();
                translators.Add(typeof(E), translator.Load(stage));
            }
            
            Translator<StageTranslator, StageInfo>();
            Translator<TickerTranslator, TickerInfo>();
            Translator<SeatTranslator, SeatInfo>();
            Translator<TagTranslator, TagInfo>();
            Translator<AttributeTranslator, AttributeInfo>();
            Translator<SpatialTranslator, SpatialInfo>();
            Translator<StateMachineTranslator, StateMachineInfo>();
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