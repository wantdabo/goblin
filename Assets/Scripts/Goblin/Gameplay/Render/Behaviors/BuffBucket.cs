using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Render.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Behaviors
{
    /// <summary>
    /// Buff 桶
    /// </summary>
    public class BuffBucket : Behavior
    {
        /// <summary>
        /// Buff 信息字典
        /// </summary>
        private Dictionary<(uint, uint), (uint id, byte state, uint layer, uint maxlayer, uint from)> buffdict { get; set; } = new();
        
        /// <summary>
        /// 获取 Buff 信息
        /// </summary>
        /// <param name="id">BuffID</param>
        /// <param name="from">来源/ActorID</param>
        /// <returns>Buff 信息</returns>
        public (uint id, byte state, uint layer, uint maxlayer, uint from) Get(uint id, uint from)
        {
            return buffdict.GetValueOrDefault((id, from));
        }

        /// <summary>
        /// 设置 Buff 信息
        /// </summary>
        /// <param name="buff">Buff 信息</param>
        public void Set((uint id, byte type, byte state, uint layer, uint maxlayer, uint from) buff)
        {
            var key = (buff.id, buff.from);
            if (buffdict.ContainsKey(key)) buffdict.Remove(key);

            buffdict.Add(key, (buff.id, buff.state, buff.layer, buff.maxlayer, buff.from));
        }
    }
}
