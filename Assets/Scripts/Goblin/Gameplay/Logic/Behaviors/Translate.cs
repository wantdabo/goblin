using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Translators;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Behaviors
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

            void Translator<T, E>() where T : Translator, new() where E : BehaviorInfo
            {
                var translator = ObjectCache.Get<T>();
                translators.Add(typeof(E), translator.Load(stage));
            }
            
            translators = ObjectCache.Get<Dictionary<Type, Translator>>();
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

        /// <summary>
        /// 执行翻译
        /// </summary>
        public void Execute()
        {
            var info = stage.GetBehaviorInfo<StageInfo>(stage.sa);
            // 先处理StageInfo（通常需要在其他翻译之前完成）
            if (translators.TryGetValue(typeof(StageInfo), out var translator)) translator.Translate(info);
    
            // 并行处理各种类型的行为信息
            Parallel.ForEach(info.behaviorinfos, kv => 
            {
                if (translators.TryGetValue(kv.Key, out var translator))
                {
                    // 对每个类型的所有行为信息进行处理
                    foreach (var behaviorinfo in kv.Value)
                    {
                        translator.Translate(behaviorinfo);
                    }
                }
            });
        }

        protected override void OnEndTick()
        {
            Execute();
        }
    }
}