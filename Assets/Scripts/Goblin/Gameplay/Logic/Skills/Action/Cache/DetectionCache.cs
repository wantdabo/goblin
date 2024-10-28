using Goblin.Gameplay.Logic.Skills.Action.Cache.Common;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Skills.Action.Cache
{
    /// <summary>
    /// 碰撞检测缓存
    /// </summary>
    public class DetectionCache : SkillActionCache
    {
        public Dictionary<uint, uint> detected { get; private set; } = new();

        public uint Query(uint id)
        {
            return detected.GetValueOrDefault(id);
        }
        
        public void Stamp(uint id)
        {
            if (detected.TryGetValue(id, out var cnt)) detected.Remove(id);
            detected.Add(id, ++cnt);
        }

        public override void OnReset()
        {
            base.OnReset();
            detected.Clear();
        }
    }
}
