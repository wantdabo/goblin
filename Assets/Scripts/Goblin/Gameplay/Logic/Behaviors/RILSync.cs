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
        private Dictionary<Type, List<Translator>> translatordict { get; set; }
        /// <summary>
        /// 渲染指令的哈希值缓存
        /// </summary>
        private ConcurrentDictionary<(ulong, ushort), int> hashcodedict { get; set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();
            translatordict = ObjectCache.Ensure<Dictionary<Type, List<Translator>>>();
            void Translator<T, E>() where T : Translator, new() where E : BehaviorInfo
            {
                var type = typeof(E);
                if (false == translatordict.TryGetValue(type, out var translators)) translatordict.Add(type, translators = ObjectCache.Ensure<List<Translator>>());
                var translator = ObjectCache.Ensure<T>();
                translators.Add(translator.Load(stage));
            }
            
            Translator<StageTranslator, StageInfo>();
            Translator<TickerTranslator, TickerInfo>();
            Translator<SeatTranslator, SeatInfo>();
            Translator<TagTranslator, TagInfo>();
            Translator<AttributeTranslator, AttributeInfo>();
            Translator<SpatialTranslator, SpatialInfo>();
            Translator<StateMachineTranslator, StateMachineInfo>();
            Translator<ActorTranslator, StageInfo>();
            
            hashcodedict = ObjectCache.Ensure<ConcurrentDictionary<(ulong, ushort), int>>();
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            foreach (var list in translatordict.Values)
            {
                foreach (var translator in list)
                {
                    translator.Unload();
                    ObjectCache.Set(translator);
                }
                
                list.Clear();
                ObjectCache.Set(list);
            }
            translatordict.Clear();
            ObjectCache.Set(translatordict);
            
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
            if (translatordict.TryGetValue(typeof(StageInfo), out var translators))
            {
                foreach (var translator in translators) translator.Translate(info);
            }

            // 并行处理各种类型的行为信息
            Parallel.ForEach(info.behaviorinfos, kv =>
            {
                if (translatordict.TryGetValue(kv.Key, out var translators))
                {
                    foreach (var translator in translators)
                    {
                        // 对每个类型的所有行为信息进行处理
                        foreach (var behaviorinfo in kv.Value)
                        {
                            translator.Translate(behaviorinfo);
                        }
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