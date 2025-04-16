using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Translators;
using Attribute = Goblin.Gameplay.Logic.Translators.Attribute;

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
            translators.Add(typeof(StageInfo), ObjectCache.Get<Translators.Synopsis>().Load(stage));
            translators.Add(typeof(SeatInfo), ObjectCache.Get<Translators.Seat>().Load(stage));
            translators.Add(typeof(TagInfo), ObjectCache.Get<Translators.Tag>().Load(stage));
            translators.Add(typeof(AttributeInfo), ObjectCache.Get<Translators.Attribute>().Load(stage));
            translators.Add(typeof(SpatialInfo), ObjectCache.Get<Translators.Spatial>().Load(stage));
            translators.Add(typeof(StateMachineInfo),  ObjectCache.Get<Translators.StateMachine>().Load(stage));
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