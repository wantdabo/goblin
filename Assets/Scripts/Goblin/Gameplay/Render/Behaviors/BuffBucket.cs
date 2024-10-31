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
        private Dictionary<uint, (byte state, uint layer, uint maxlayer)> buffdict { get; set; } = new();
        
        /// <summary>
        /// 获取 Buff 信息
        /// </summary>
        /// <param name="id">BuffID</param>
        /// <returns>Buff 信息</returns>
        public (byte state, uint layer, uint maxlayer) Get(uint id)
        {
            if (false == buffdict.TryGetValue(id, out var info))
            {
                info = (BUFF_STATE_DEFINE.INACTIVE, 0, 0);
                buffdict.Add(id, info);
            }
            
            return info;
        }

        /// <summary>
        /// 设置 Buff 信息
        /// </summary>
        /// <param name="id">BuffID</param>
        /// <param name="buff">Buff 信息</param>
        public void Set(uint id, (byte state, uint layer, uint maxlayer) buff)
        {
            if (buffdict.ContainsKey(id)) buffdict.Remove(id);
            
            buffdict.Add(id, buff);
        }
    }
}
