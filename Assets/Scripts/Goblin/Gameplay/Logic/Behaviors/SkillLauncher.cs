using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Flows.BehaviorInfos;
using Goblin.Gameplay.Logic.Flows.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 技能释放器
    /// </summary>
    public class SkillLauncher : Behavior<SkillLauncherInfo>
    {
        protected override void OnAssemble()
        {
            base.OnAssemble();
            stage.eventor.Listen<ActorDeadEvent>(actor.eventor, OnActorDead);
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            stage.eventor.UnListen<ActorDeadEvent>(actor.eventor, OnActorDead);
        }

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
            if (info.loadedskills.ContainsKey(skill)) throw new Exception($"skill : {skill} already loaded.");
            var skillinfo = ObjectCache.Get<SkillInfo>();
            skillinfo.skill = skill;
            skillinfo.strength = strength;
            skillinfo.cooldown = cooldown;
            skillinfo.pipelines = pipelines;
            info.loadedskills.Add(skill, skillinfo);
        }
        
        /// <summary>
        /// 卸载技能
        /// </summary>
        /// <param name="skill">技能 ID</param>
        public void Unload(uint skill)
        {
            if (false == info.loadedskills.TryGetValue(skill, out var skillinfo)) return;
            
            skillinfo.pipelines.Clear();
            ObjectCache.Set(skillinfo.pipelines);
            
            info.loadedskills.Remove(skill);
            ObjectCache.Set(skillinfo);
        }

        /// <summary>
        /// 打断技能
        /// </summary>
        public void Break()
        {
            if (false == info.casting) return;
            stage.flow.EndPipeline(info.flow);
        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skill">技能 ID</param>
        public void Launch(uint skill)
        {
            if (info.casting) return;
            if (false == info.loadedskills.TryGetValue(skill, out var skillinfo)) return;
            
            var pipelines = ObjectCache.Get<List<uint>>();
            foreach (var pipeline in skillinfo.pipelines) pipelines.Add(pipeline);
            
            // 创建管线
            info.skill = skill;
            info.flow = stage.flow.GenPipeline(actor.id, pipelines).id;
            info.casting = true;
        }

        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == info.casting) return;

            // 技能 timescale 覆盖 flow timescale
            if (actor.SeekBehaviorInfo(out TickerInfo tickerinfo) && stage.SeekBehaviorInfo(info.flow, out TickerInfo flowtickerinfo))
            {
                flowtickerinfo.timescale = tickerinfo.timescale;
            }

            if (false == info.loadedskills.TryGetValue(info.skill, out var skillinfo) || false == stage.SeekBehaviorInfo(info.flow, out CollisionInfo collisioninfo)) return;
            var damage = stage.calc.ChargeDamage(actor.id, skillinfo.strength);
            while (collisioninfo.collisions.TryDequeue(out var target))
            {
                stage.calc.ToDamage(actor.id, target, damage);
            }
        }

        private void OnActorDead(ActorDeadEvent e)
        {
            if (false == info.casting) return;
            if (e.id != info.id) return;
            
            // 技能管线 Actor 被移除, 结束了
            info.skill = 0;
            info.flow = 0;
            info.casting = false;
        }
    }
}