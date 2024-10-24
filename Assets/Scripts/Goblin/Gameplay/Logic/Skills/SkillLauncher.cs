using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Skills
{
    /// <summary>
    /// 技能释放器
    /// </summary>
    public class SkillLauncher : Behavior<Translator>
    {
        /// <summary>
        /// 技能列表
        /// </summary>
        public List<uint> skills { get; private set; } = new();
        /// <summary>
        /// 技能字典
        /// </summary>
        private Dictionary<uint, SkillPipeline> skilldict = new();
        /// <summary>
        /// 技能释放解析
        /// </summary>
        public SkillCaster caster { get; private set; }
        /// <summary>
        /// 进行中的技能 (进行中, 技能 ID)
        /// </summary>
        public (bool playing, uint skill) launching
        {
            get
            {
                foreach (uint skill in skills)
                {
                    var pipeline = Get(skill);
                    if (SkillPipelineStateDef.None != pipeline.state) return (true, skill);
                }

                return (false, default);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
            caster = AddComp<SkillCaster>();
            caster.launcher = this;
            caster.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
        }
        
        /// <summary>
        /// 技能释放
        /// </summary>
        /// <param name="id">技能 ID</param>
        /// <returns>YES/NO</returns>
        public bool Launch(uint id)
        {
            var pipeline = Get(id);
            if (null == pipeline) return false;

            return pipeline.Launch();
        }

        /// <summary>
        /// 获取技能管线
        /// </summary>
        /// <param name="id">技能 ID</param>
        /// <returns>技能管线</returns>
        public SkillPipeline Get(uint id)
        {
            return skilldict.GetValueOrDefault(id);
        }
        
        /// <summary>
        /// 加载技能管线
        /// </summary>
        /// <param name="id">技能 ID</param>
        public void Load(uint id)
        {
            var pipeline = Get(id);
            if (null != pipeline) return;

            pipeline = AddComp<SkillPipeline>();
            pipeline.id = id;
            pipeline.launcher = this;
            pipeline.Create();
            skills.Add(id);
            skilldict.Add(id, pipeline);
        }

        private void OnFPTick(FPTickEvent e)
        {
            foreach (var pipeline in skilldict.Values) pipeline.OnFPTick(e.tick);
            caster.OnFPTick(e.tick);
        }
    }
}
