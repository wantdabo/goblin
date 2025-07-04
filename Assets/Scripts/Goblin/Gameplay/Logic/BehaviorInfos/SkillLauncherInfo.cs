using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 技能信息
    /// </summary>
    public struct SkillInfo
    {
        /// <summary>
        /// 技能 ID
        /// </summary>
        public uint skill { get; set; }
        /// <summary>
        /// 技能的伤害强度
        /// </summary>
        public FP strength { get; set; }
        /// <summary>
        /// 技能冷却时间
        /// </summary>
        public FP cooldown { get; set; }
        /// <summary>
        /// 管线列表
        /// </summary>
        public List<uint> pipelines { get; set; }
    }

    /// <summary>
    /// 技能释放器信息
    /// </summary>
    public class SkillLauncherInfo : BehaviorInfo
    {
        /// <summary>
        /// 正在进行的技能 ID
        /// </summary>
        public uint skill { get; set; }
        /// <summary>
        /// 技能管线
        /// </summary>
        public ulong flow { get; set; }
        /// <summary>
        /// 是否有技能在释放中
        /// </summary>
        public bool casting { get; set; }
        /// <summary>
        /// 技能列表
        /// </summary>
        public List<uint> loadedskills { get; set; }
        /// <summary>
        /// 技能字典, 键为技能 ID, 值为技能信息
        /// </summary>
        public Dictionary<uint, SkillInfo> loadedskilldict { get; set; }

        protected override void OnReady()
        {
            skill = 0;
            flow = 0;
            casting = false;
            loadedskills = ObjectCache.Ensure<List<uint>>();
            loadedskilldict = ObjectCache.Ensure<Dictionary<uint, SkillInfo>>();
        }

        protected override void OnReset()
        {
            skill = 0;
            flow = 0;
            casting = false;
            loadedskills.Clear();
            ObjectCache.Set(loadedskills);
            foreach (var kv in loadedskilldict)
            {
                kv.Value.pipelines.Clear();
                ObjectCache.Set(kv.Value.pipelines);
            }
            loadedskilldict.Clear();
            ObjectCache.Set(loadedskilldict);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<SkillLauncherInfo>();
            clone.Ready(actor);
            clone.skill = skill;
            clone.flow = flow;
            clone.casting = casting;
            foreach (var skillid in loadedskills) clone.loadedskills.Add(skillid);
            foreach (var kv in loadedskilldict)
            {
                var skillinfo = new SkillInfo
                {
                    skill = kv.Value.skill,
                    strength = kv.Value.strength,
                    cooldown = kv.Value.cooldown,
                    pipelines = ObjectCache.Ensure<List<uint>>()
                };
                foreach (var pipeline in kv.Value.pipelines) skillinfo.pipelines.Add(pipeline);
                
                clone.loadedskilldict.Add(kv.Key, skillinfo);
            }

            return clone;
        }
    }
}