using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Logic.Translators;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Behaviors.Sa
{
    /// <summary>
    /// RIL/渲染指令同步
    /// </summary>
    public class RILSync : Behavior
    {
        /// <summary>
        /// RIL 翻译器集合 (Translator 类型 -> Translator 列表)
        /// </summary>
        private Dictionary<Type, List<Translator>> translatordict { get; set; }
        /// <summary>
        /// 渲染指令差异队列
        /// </summary>
        private Queue<IRIL_DIFF> diffqueue { get; set; }
        /// <summary>
        /// 渲染指令事件队列
        /// </summary>
        private Queue<IRIL_EVENT> eventqueue { get; set; }
        /// <summary>
        /// 同步锁对象
        /// </summary>
        private object @lock = new();
        /// <summary>
        /// 渲染指令的哈希值缓存
        /// </summary>
        private Dictionary<(ulong, ushort), int> hashcodedict { get; set; }

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
            Translator<MotionTranslator, MotionInfo>();
            Translator<FacadeTranslator, FacadeInfo>();
            
            diffqueue = ObjectCache.Ensure<Queue<IRIL_DIFF>>();
            eventqueue = ObjectCache.Ensure<Queue<IRIL_EVENT>>();
            hashcodedict = ObjectCache.Ensure<Dictionary<(ulong, ushort), int>>();
            
            stage.eventor.Listen<ActorRmvEvent>(this, OnActorRmv);
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

            while (diffqueue.TryDequeue(out var diff))
            {
                diff.Reset();
                ObjectCache.Set(diff);
            }
            diffqueue.Clear();
            
            while (eventqueue.TryDequeue(out var e))
            {
                ObjectCache.Set(e);
            }
            eventqueue.Clear();
            
            hashcodedict.Clear();
            ObjectCache.Set(hashcodedict);
            
            stage.eventor.UnListen<ActorRmvEvent>(this, OnActorRmv);
        }
        
        /// <summary>
        /// 缓存渲染指令的哈希值
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="id">RIL ID</param>
        /// <param name="hashcode">RIL/BehaviorInfo 哈希值</param>
        public void CacheHashCode(ulong actor, ushort id, int hashcode)
        {
            lock (@lock)
            {
                var key = (actor, id);
                if (hashcodedict.ContainsKey(key)) hashcodedict.Remove(key);
                hashcodedict.Add(key, hashcode);
            }
        }

        /// <summary>
        /// 移除渲染指令的哈希值
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="id">RIL ID</param>
        private void RmvHashCode(ulong actor, ushort id)
        {
            lock (@lock)
            {
                var key = (actor, id);
                if (hashcodedict.ContainsKey(key)) hashcodedict.Remove(key);
            }
        }

        /// <summary>
        /// 移除 Actor 的所有渲染指令哈希值
        /// </summary>
        /// <param name="actor">ActorID</param>
        private void RmvHashCode(ulong actor)
        {
            lock (@lock)
            {
                var list = ObjectCache.Ensure<List<(ulong, ushort)>>();
                foreach (var kv in hashcodedict) if (kv.Key.Item1 == actor) list.Add(kv.Key);
                foreach (var key in list) hashcodedict.Remove(key);
                list.Clear();
                ObjectCache.Set(list);
            }
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
            diffqueue.Enqueue(diff);
        }

        /// <summary>
        /// 发送渲染指令事件
        /// </summary>
        /// <param name="e">事件指令</param>
        public void Send(IRIL_EVENT e)
        {
            eventqueue.Enqueue(e);
        }
        
        /// <summary>
        /// 发送丢失的渲染指令 (RIL_LOSS)
        /// </summary>
        private void SendLossRIL()
        {
            foreach (var rmvbehaviorinfo in stage.cache.rmvbehaviorinfos)
            {
                if (stage.SeekBehavior(rmvbehaviorinfo.actor, out Tag tag) && tag.Get(TAG_DEFINE.ACTOR_TYPE, out var val))
                {
                    if (val == ACTOR_DEFINE.FLOW) continue;
                }
                
                if (false == translatordict.TryGetValue(rmvbehaviorinfo.GetType(), out var translators)) continue;
                foreach (var translator in translators)
                {
                    var ril =  RILCache.Ensure<RIL_LOSS>();
                    ril.Ready(rmvbehaviorinfo.actor, 0);
                    ril.loss = translator.id;
                    
                    translator.RmvOnceActor(rmvbehaviorinfo.actor);
                    if (hashcodedict.ContainsKey((rmvbehaviorinfo.actor, translator.id))) 
                    {
                        hashcodedict.Remove((rmvbehaviorinfo.actor, translator.id), out _);
                    }

                    RmvHashCode(ril.actor, ril.loss);
                    Send(ril);
                }
            }
        }

        /// <summary>
        /// 执行翻译
        /// </summary>
        public void Translate()
        {
            var info = stage.GetBehaviorInfo<StageInfo>(stage.sa);
            // 先处理StageInfo（通常需要在其他翻译之前完成）
            if (translatordict.TryGetValue(typeof(StageInfo), out var translators))
            {
                foreach (var translator in translators) translator.Translate(info);
            }

            SendLossRIL();
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
                            if (stage.cache.rmvactors.Contains(behaviorinfo.actor)) continue;
                            if (stage.cache.rmvbehaviorinfos.Contains(behaviorinfo)) continue;
                            if (stage.SeekBehavior(behaviorinfo.actor, out Tag tag) && tag.Get(TAG_DEFINE.ACTOR_TYPE, out var val))
                            {
                                if (val == ACTOR_DEFINE.FLOW) continue;
                            }
            
                            translator.Translate(behaviorinfo);
                        }
                    }
                }
            });

            // 处理渲染指令差异
            while (diffqueue.TryDequeue(out var diff))
            {
                var clone = diff.Clone(RILCache.Ensure(diff.GetType()) as IRIL_DIFF);
                diff.Reset();
                ObjectCache.Set(diff);
                
                stage.ondiff?.Invoke(clone);
            }
            
            // 处理渲染指令事件
            while (eventqueue.TryDequeue(out var e))
            {
                var clone = e.Clone(RILCache.Ensure(e.GetType()) as IRIL_EVENT);
                ObjectCache.Set(e);
                
                stage.onevent?.Invoke(clone);
            }
        }

        protected override void OnEndTick()
        {
            Translate();
        }
        
        private void OnActorRmv(ActorRmvEvent e)
        {
            RmvHashCode(e.actor);
        }
    }
}