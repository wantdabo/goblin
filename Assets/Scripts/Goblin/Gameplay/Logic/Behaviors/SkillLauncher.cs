using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
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
        /// <param name="id">技能 ID</param>
        /// <param name="strength">技能强度</param>
        /// <param name="cooldown">技能冷却</param>
        /// <param name="pipelines">管线列表</param>
        /// <exception cref="Exception">技能重复加载</exception>
        public void Load(uint id, FP strength, FP cooldown, List<uint> pipelines)
        {
            if (info.loadedskills.ContainsKey(id)) throw new Exception($"skill : {id} already loaded.");
            var skillinfo = ObjectCache.Get<SkillInfo>();
            skillinfo.skillid = id;
            skillinfo.strength = strength;
            skillinfo.cooldown = cooldown;
            skillinfo.pipelines = pipelines;
            info.loadedskills.Add(id, skillinfo);
        }
        
        /// <summary>
        /// 卸载技能
        /// </summary>
        /// <param name="id">技能 ID</param>
        public void Unload(uint id)
        {
            if (false == info.loadedskills.TryGetValue(id, out var skillinfo)) return;
            
            skillinfo.pipelines.Clear();
            ObjectCache.Set(skillinfo.pipelines);
            
            info.loadedskills.Remove(id);
            ObjectCache.Set(skillinfo);
        }

        /// <summary>
        /// 打断技能
        /// </summary>
        public void Break()
        {
            if (false == info.casting) return;
            stage.flow.EndPipeline(info.pipeline);
        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="id">技能 ID</param>
        public void Launch(uint id)
        {
            if (info.casting) return;
            if (false == info.loadedskills.TryGetValue(id, out var skillinfo)) return;
            
            var pipelines = ObjectCache.Get<List<uint>>();
            foreach (var pipeline in skillinfo.pipelines) pipelines.Add(pipeline);
            
            // 创建管线
            var pipelineactor = stage.flow.GenPipeline(actor.id, pipelines);
            info.skill = id;
            info.pipeline = pipelineactor.id;
            info.casting = true;
        }

        private void OnActorDead(ActorDeadEvent e)
        {
            if (false == info.casting) return;
            if (e.id != info.id) return;
            
            // 技能管线 Actor 被移除, 结束了
            info.skill = 0;
            info.pipeline = 0;
            info.casting = false;
        }
    }
}