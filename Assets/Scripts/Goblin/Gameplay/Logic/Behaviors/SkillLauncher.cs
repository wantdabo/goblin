using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 技能释放器
    /// </summary>
    public class SkillLauncher : Behavior<SkillLauncherInfo>
    {
        /// <summary>
        /// 装载技能
        /// </summary>
        /// <param name="skill">技能 ID</param>
        /// <param name="strength">技能强度</param>
        /// <param name="cooldown">技能冷却</param>
        /// <param name="pipelines">管线列表</param>
        /// <exception cref="Exception">技能重复加载</exception>
        public void Load(uint skill, FP strength, FP cooldown, List<uint> pipelines)
        {
            if (info.loadedskilldict.ContainsKey(skill)) throw new Exception($"skill : {skill} already loaded.");
            var skillinfo = new SkillInfo
            {
                skill = skill,
                strength = strength,
                cooldown = cooldown,
                pipelines = pipelines
            };
            info.loadedskills.Add(skill);
            info.loadedskilldict.Add(skill, skillinfo);
        }

        /// <summary>
        /// 卸载技能
        /// </summary>
        /// <param name="skill">技能 ID</param>
        public void Unload(uint skill)
        {
            if (false == info.loadedskilldict.TryGetValue(skill, out var skillinfo)) return;
            
            skillinfo.pipelines.Clear();
            ObjectCache.Set(skillinfo.pipelines);
            
            info.loadedskills.Remove(skill);
            info.loadedskilldict.Remove(skill);
        }

        /// <summary>
        /// 打断技能
        /// </summary>
        public void Break()
        {
            if (false == info.casting) return;
            info.casting = false;
            stage.flow.EndPipeline(info.flow);
        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skill">技能 ID</param>
        public void Launch(uint skill)
        {
            if (info.casting) return;
            if (false == info.loadedskilldict.TryGetValue(skill, out var skillinfo)) return;

            // 创建管线
            info.skill = skill;
            info.flow = stage.flow.GenPipeline(actor, skillinfo.pipelines);
            info.casting = true;
        }

        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            
            if (false == info.casting) return;

            // 碰撞检测
            if (info.loadedskilldict.TryGetValue(info.skill, out var skillinfo) && stage.SeekBehaviorInfo(info.flow, out FlowCollisionHurtInfo collisionhurt))
            {
                var damage = stage.attrc.ChargeDamage(actor, skillinfo.strength);
                while (collisionhurt.targets.TryDequeue(out var target))
                {
                    if (actor == target.actor) continue;
                    stage.attrc.ToDamage(actor, target.actor, damage);
                }
            }

            // 技能管线结束检查
            if (false == stage.flow.CheckFlowActive(info.flow))
            {
                info.skill = 0;
                info.flow = 0;
                info.casting = false;
            }
            
            // 切换状态机中状态
            if (stage.SeekBehavior(actor, out StateMachine statemachine))
            {
                if (info.casting && STATE_DEFINE.CASTING != statemachine.info.current)
                {
                    statemachine.TryChangeState(STATE_DEFINE.CASTING);
                }
                else if (false == info.casting && STATE_DEFINE.CASTING == statemachine.info.current)
                {
                    statemachine.ChangeState(STATE_DEFINE.NONE);
                }
            }
        }
    }
}