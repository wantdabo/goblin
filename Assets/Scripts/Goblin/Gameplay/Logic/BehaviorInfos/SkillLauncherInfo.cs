using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 技能信息
    /// </summary>
    public class SkillInfo
    {
        /// <summary>
        /// 技能 ID
        /// </summary>
        public uint skillid { get; set; }
        /// <summary>
        /// 技能强度
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
        /// 正在进行的技能管线 ActorID
        /// </summary>
        public ulong pipeline { get; set; }
        /// <summary>
        /// 是否有技能在释放中
        /// </summary>
        public bool casting { get; set; }
        /// <summary>
        /// 技能字典, 键为技能 ID, 值为技能信息
        /// </summary>
        public Dictionary<uint, SkillInfo> loadedskills { get; set; }

        protected override void OnReady()
        {
            skill = 0;
            pipeline = 0;
            casting = false;
            loadedskills = ObjectCache.Get<Dictionary<uint, SkillInfo>>();
        }

        protected override void OnReset()
        {
            skill = 0;
            pipeline = 0;
            casting = false;
            foreach (var kv in loadedskills)
            {
                kv.Value.pipelines.Clear();
                ObjectCache.Set(kv.Value.pipelines);
            }
            loadedskills.Clear();
            ObjectCache.Set(loadedskills);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<SkillLauncherInfo>();
            clone.Ready(id);
            clone.skill = skill;
            clone.pipeline = pipeline;
            clone.casting = casting;
            foreach (var kv in loadedskills)
            {
                var skillinfo = ObjectCache.Get<SkillInfo>();
                skillinfo.skillid = kv.Value.skillid;
                skillinfo.strength = kv.Value.strength;
                skillinfo.cooldown = kv.Value.cooldown;
                skillinfo.pipelines = ObjectCache.Get<List<uint>>();
                foreach (var pipeline in kv.Value.pipelines) skillinfo.pipelines.Add(pipeline);
                clone.loadedskills.Add(kv.Key, skillinfo);
            }

            return clone;
        }
    }
}