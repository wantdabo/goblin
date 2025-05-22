using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// RIL/渲染指令同步
    /// </summary>
    public class RILSync : Behavior
    {
        /// <summary>
        /// 渲染指令的哈希值缓存
        /// </summary>
        private ConcurrentDictionary<(ulong, ushort), int> hashcodedict { get; set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();
            hashcodedict = ObjectCache.Ensure<ConcurrentDictionary<(ulong, ushort), int>>();
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
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
        /// 刷新所有渲染指令
        /// </summary>
        public void RefreshRILS()
        {
            hashcodedict.Clear();
            // 重新推一次最新的状态到渲染层
            if (actor.SeekBehavior(out Translate translate)) translate.Execute();
        }
    }
}