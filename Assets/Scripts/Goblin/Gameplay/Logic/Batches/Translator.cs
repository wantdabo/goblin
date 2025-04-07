using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Translators;
using Attribute = Goblin.Gameplay.Logic.Translators.Attribute;

namespace Goblin.Gameplay.Logic.Batches
{
    /// <summary>
    /// 翻译 RIL 渲染指令, 所有 Stage BehaviorInfo 来生成渲染指令
    /// </summary>
    public class Translator : Batch
    {
        /// <summary>
        /// RIL 翻译器集合
        /// </summary>
        public Dictionary<Type, Logic.Translators.Common.Translator> translators { get; set; }
        
        protected override void OnLoad()
        {
            base.OnLoad();
            translators = ObjectCache.Get<Dictionary<Type, Logic.Translators.Common.Translator>>();
            translators.Add(typeof(AttributeInfo), ObjectCache.Get<Attribute>().Load(stage));
            translators.Add(typeof(SpatialInfo), ObjectCache.Get<Spatial>().Load(stage));
            translators.Add(typeof(StateMachineInfo),  ObjectCache.Get<StateMachine>().Load(stage));
        }

        protected override void OnUnload()
        {
            base.OnUnload();
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
            foreach (var kv in stage.info.behaviorinfodict)
            {
                foreach (var kv2 in kv.Value)
                {
                    if (false == translators.TryGetValue(kv2.Key, out var translator)) continue;
                    translator.Translate(kv2.Value);
                }
            }
        }
    }
}