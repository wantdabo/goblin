using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Logic.Translators;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// RIL/渲染指令同步
    /// </summary>
    public class RILSync : Behavior
    {
        /// <summary>
        /// RIL 翻译器集合
        /// </summary>
        private Dictionary<Type, Translator> translators { get; set; }
        /// <summary>
        /// 渲染指令的哈希值缓存
        /// </summary>
        private ConcurrentDictionary<(ulong, ushort), int> hashcodedict { get; set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();
            void Translator<T, E>() where T : Translator, new() where E : BehaviorInfo
            {
                var translator = ObjectCache.Ensure<T>();
                translators.Add(typeof(E), translator.Load(stage));
            }
            
            translators = ObjectCache.Ensure<Dictionary<Type, Translator>>();
            Translator<StageTranslator, StageInfo>();
            Translator<TickerTranslator, TickerInfo>();
            Translator<SeatTranslator, SeatInfo>();
            Translator<TagTranslator, TagInfo>();
            Translator<AttributeTranslator, AttributeInfo>();
            Translator<SpatialTranslator, SpatialInfo>();
            Translator<StateMachineTranslator, StateMachineInfo>();
            
            hashcodedict = ObjectCache.Ensure<ConcurrentDictionary<(ulong, ushort), int>>();
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
            
            hashcodedict.Clear();
            ObjectCache.Set(hashcodedict);
        }
        
        /// <summary>
        /// 缓存渲染指令的哈希值
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="id">RIL ID</param>
        /// <param name="hashcode">RIL/BehaviorInfo 哈希值</param>
        public void CacheHashCode(ulong actor, ushort id, int hashcode)
        {
            var key = (actor, id);
            if (hashcodedict.ContainsKey(key)) hashcodedict.Remove(key, out _);
            hashcodedict.TryAdd(key, hashcode);
        }

        /// <summary>
        /// 查询渲染指令的哈希值
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="id">RIL ID</param>
        /// <returns>RIL/BehaviorInfo 哈希值</returns>
        public int Query(ulong actor, ushort id)
        {
            if (false == hashcodedict.TryGetValue((actor, id), out int value)) return -1;

            return value;
        }

        /// <summary>
        /// 发送渲染指令
        /// </summary>
        /// <param name="ril">渲染指令</param>
        public void Send(IRIL ril)
        {
            // 发送渲染指令
            stage.onril?.Invoke(ril);
        }

        /// <summary>
        /// 发送差异渲染指令
        /// </summary>
        /// <param name="diff">差异指令</param>
        public void Send(IRIL_DIFF diff)
        {
            // 发送差异渲染指令
            stage.ondiff?.Invoke(diff);
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