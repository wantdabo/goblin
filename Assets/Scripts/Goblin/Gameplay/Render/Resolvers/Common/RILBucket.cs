using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Cross;
using Goblin.Gameplay.Render.Resolvers.Enchants;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    /// <summary>
    /// RIL 指令事件
    /// </summary>
    public struct RILEvent : IEvent
    {
        /// <summary>
        /// 渲染状态
        /// </summary>
        public IRIL ril { get; set; }
    }

    /// <summary>
    /// 数据状态桶, 存储着渲染层的所有数据状态
    /// </summary>
    public class RILBucket : Comp
    {
        /// <summary>
        /// 世界
        /// </summary>
        public World world { get; private set; }
        /// <summary>
        /// 事件订阅派发者
        /// </summary>
        public Eventor eventor { get; private set; }
        /// <summary>
        /// 渲染状态集合
        /// </summary>
        private Dictionary<ulong, Dictionary<Type, IRIL>> rildict { get; set; }
        /// <summary>
        /// 渲染状态集合
        /// </summary>
        private Dictionary<ulong, Dictionary<ushort, IRIL>> rilidsdict { get; set; }
        /// <summary>
        /// 渲染状态交叉集合
        /// </summary>
        private Dictionary<ushort, RILCross> crossdict { get; set; }
        /// <summary>
        /// Agent 赋能集合
        /// </summary>
        private Dictionary<ushort, List<AgentEnchant>> enchantdict { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
            
            rildict = ObjectPool.Ensure<Dictionary<ulong, Dictionary<Type, IRIL>>>();
            rilidsdict = ObjectPool.Ensure<Dictionary<ulong, Dictionary<ushort, IRIL>>>();
            Cross();
            Enchants();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LossAllRIL();
            ObjectPool.Set(rildict);
            ObjectPool.Set(rilidsdict);
            
            crossdict.Clear();
            ObjectPool.Set(crossdict);

            foreach (var kv in enchantdict)
            {
                kv.Value.Clear();
                ObjectPool.Set(kv.Value);
            }
            enchantdict.Clear();
            ObjectPool.Set(enchantdict);
        }

        /// <summary>
        /// 初始化数据状态桶
        /// </summary>
        /// <param name="world">世界</param>
        /// <returns>初始化数据状态桶</returns>
        public RILBucket Initialize(World world)
        {
            this.world = world;

            return this;
        }

        /// <summary>
        /// 初始化 RIL 合并器
        /// </summary>
        private void Cross()
        {
            crossdict = ObjectPool.Ensure<Dictionary<ushort, RILCross>>();
            void Cross<T>(ushort id) where T : RILCross, new()
            {
                var cross = AddComp<T>().Initialize(this);
                cross.Create();
                crossdict.Add(id, cross);
            }
            
            Cross<TagCross>(RIL_DEFINE.TAG);
            Cross<ActorCross>(RIL_DEFINE.ACTOR);
        }

        /// <summary>
        /// 初始化代理赋能
        /// </summary>
        private void Enchants()
        {
            enchantdict = ObjectPool.Ensure<Dictionary<ushort, List<AgentEnchant>>>();
            void Enchant<T>(ushort id) where T : AgentEnchant, new()
            {
                var enchant = AddComp<T>().Initialize(this);
                enchant.Create();
                if (false == enchantdict.TryGetValue(id, out var list)) enchantdict.Add(id, list = ObjectPool.Ensure<List<AgentEnchant>>());
                list.Add(enchant);
            }
            
            Enchant<NodeEnchant>(RIL_DEFINE.SPATIAL);
            Enchant<ModelEnchant>(RIL_DEFINE.TAG);
            Enchant<AnimationEnchant>(RIL_DEFINE.TAG);
        }

        /// <summary>
        /// 清除所有的状态
        /// </summary>
        public void LossAllRIL()
        {
            foreach (var rils in rildict.Values)
            {
                rils.Clear();
                ObjectPool.Set(rils);
            }
            rildict.Clear();
            rilidsdict.Clear();
        }

        /// <summary>
        /// 清除 Actor 的所有状态
        /// </summary>
        /// <param name="actor">ActorID</param>
        public void LossRIL(ulong actor)
        {
            if (false == rildict.TryGetValue(actor, out var dict)) return;
            
            foreach (var kv in dict)
            {
                var ril = kv.Value;
                ril.Reset();
                RILCache.Set(ril);
            }
            rildict.Remove(actor);
            dict.Clear();
            ObjectPool.Set(dict);

            if (rilidsdict.TryGetValue(actor, out var iddict))
            {
                rilidsdict.Remove(actor);
                iddict.Clear();
                ObjectPool.Set(iddict);
            }
        }

        /// <summary>
        /// 获取所有的状态
        /// </summary>
        /// <param name="rils">数据状态列表</param>
        /// <typeparam name="T">数据状态类型</typeparam>
        /// <returns>YES/NO</returns>
        public bool SeekRILS<T>(out List<T> rils) where T : IRIL, new()
        {
            rils = GetRILS<T>();
            
            return null != rils && 0 != rils.Count;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="ril">数据状态</param>
        /// <typeparam name="T">数据状态类型</typeparam>
        /// <returns>YES/NO</returns>
        public bool SeekRIL<T>(ulong actor, out T ril) where T : IRIL
        {
            ril = GetRIL<T>(actor);
            
            return null != ril;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="id">RIL ID</param>
        /// <param name="actor">ActorID</param>
        /// <param name="ril">数据状态</param>
        /// <returns>YES/NO</returns>
        public bool SeekRIL(ushort id, ulong actor, out IRIL ril)
        {
            ril = GetRIL(id, actor);
            
            return null != ril;
        }

        /// <summary>
        /// 获取所有的状态
        /// </summary>
        /// <typeparam name="T">数据状态类型</typeparam>
        /// <returns>数据状态列表</returns>
        public List<T> GetRILS<T>() where T : IRIL
        {
            var type = typeof(T);
            var rils = ObjectPool.Ensure<List<T>>();
            foreach (var kv in rildict)
            {
                if (false == kv.Value.ContainsKey(type)) continue;
                if (false == kv.Value.TryGetValue(type, out var ril)) continue;
                rils.Add((T)ril);
            }

            // 没有找到
            if (0 == rils.Count)
            {
                ObjectPool.Set(rils);

                return default;
            }

            return rils;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <typeparam name="T">数据状态类型</typeparam>
        /// <returns>数据状态</returns>
        public T GetRIL<T>(ulong actor) where T : IRIL
        {
            if (false == rildict.TryGetValue(actor, out var dict)) return default;
            if (false == dict.TryGetValue(typeof(T), out var result)) return default;
            return result as T;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="id">RIL ID</param>
        /// <param name="actor">ActorID</param>
        /// <returns name="ril">数据状态</returns>
        public IRIL GetRIL(ushort id, ulong actor)
        {
            if (false == rilidsdict.TryGetValue(actor, out var dict)) return default;
            if (false == dict.TryGetValue(id, out var result)) return default;
            
            return result;
        }

        /// <summary>
        /// 合并状态
        /// </summary>
        /// <param name="diff">状态差异</param>
        public void SetDiff(IRIL_DIFF diff)
        {
            if (SeekRIL(diff.id, diff.actor, out var ril) && crossdict.TryGetValue(diff.id, out var cross))
            {
                switch (diff.token)
                {
                    case RIL_DEFINE.DIFF_DEL:
                        cross.HasDel(ril, diff);
                        break;
                    case RIL_DEFINE.DIFF_NEW:
                        cross.HasNew(ril, diff);
                        break;
                }
            }

            diff.Reset();
            RILCache.Set(diff);
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="ril">渲染状态</param>
        public void SetRIL(IRIL ril)
        {
            // 渲染状态
            var type = ril.GetType();
            if (false == rildict.TryGetValue(ril.actor, out var dict)) rildict.Add(ril.actor, dict = ObjectPool.Ensure<Dictionary<Type, IRIL>>());
            if (false == rilidsdict.TryGetValue(ril.actor, out var iddict)) rilidsdict.Add(ril.actor, iddict = ObjectPool.Ensure<Dictionary<ushort, IRIL>>());
            if (dict.TryGetValue(type, out var oldril))
            {
                if (ril.hashcode.Equals(oldril.hashcode))
                {
                    ril.Reset();
                    RILCache.Set(ril);
                    return;
                }
                
                oldril.Reset();
                RILCache.Set(oldril);
                dict.Remove(type);
                iddict.Remove(ril.id);
            }
            
            dict.Add(type, ril);
            iddict.Add(ril.id, ril);
            eventor.Tell(new RILEvent { ril = ril });
            RILDispatch(ril);
        }

        /// <summary>
        /// 渲染指令状态推送至 Agent 处理
        /// </summary>
        /// <param name="ril">渲染指令状态</param>
        private void RILDispatch(IRIL ril)
        {
            // Enchant 分发 RIL
            if (enchantdict.TryGetValue(ril.id, out var list)) foreach (var enchant in list) enchant.DoRIL(ril);
            
            // Agent 分发 RIL
            var dict= world.GetAgents(ril.actor);
            if (null != dict) foreach (var kv in dict) kv.Value.DoRIL(ril);
        }
    }
}