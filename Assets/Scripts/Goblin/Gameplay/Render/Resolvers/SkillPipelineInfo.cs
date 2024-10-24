using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Skills;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Effects;
using MessagePack;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 技能管线解释器
    /// </summary>
    public class SkillPipelineInfo : Resolver<RIL_SKILLPIPELINE_INFO>
    {
        public override ushort id => RILDef.SKILLPIPELINE_INFO;
        
        private Node node { set; get; }
        
        private AnimancerAnimation animation { get; set; }
        
        /// <summary>
        /// 技能行为数据列表
        /// </summary>
        private Dictionary<uint, List<SkillActionData>> skillActionDatas { get; set; } = new();
        
        /// <summary>
        /// 技能特效进行中列表
        /// </summary>
        private Dictionary<uint, Dictionary<string, VFXController>> effdict = new();

        protected override void OnAwake(uint frame, RIL_SKILLPIPELINE_INFO ril)
        {
            node = actor.EnsureBehavior<Node>();
            animation = actor.EnsureBehavior<AnimancerAnimation>();
        }

        protected override void OnResolve(uint frame, RIL_SKILLPIPELINE_INFO ril)
        {
            if (SkillPipelineStateDef.End == ril.state || SkillPipelineStateDef.None == ril.state)
            {
                if (effdict.TryGetValue(ril.skillid, out var effs))
                {
                    foreach (var eff in effs.Values) eff.Stop();
                    effs.Clear();
                }
                return;
            }

            ReadyOrInitialize(ril.skillid);
            var f = Mathf.Floor(ril.frame * GameDef.SP_DATA_LOGIC_FRAME_SCALE);
            if (skillActionDatas.TryGetValue(ril.skillid, out var actions))
            {
                foreach (var action in actions)
                {
                    var between = (f >= action.sframe && f <= action.eframe);
                    switch (action.id)
                    {
                        case SkillActionDef.ANIMATION:
                            if (false == between) break;
                            var animationData = (AnimationActionData)action;
                            animation.Play(animationData.name, f / action.eframe);
                            break;
                        case SkillActionDef.EFFECT:
                            if (false == effdict.TryGetValue(ril.skillid, out var effs))
                            {
                                effs = new();
                                effdict.Add(ril.skillid, effs);
                            }

                            EffectActionData effectData = (EffectActionData)action;
                            if (false == between)
                            {
                                if (effs.TryGetValue(effectData.res, out var e))
                                {
                                    e.Stop();
                                    effs.Remove(effectData.res);
                                }
                                break;
                            }
                            bool first = false;
                            if (false == effs.TryGetValue(effectData.res, out var eff))
                            {
                                eff = actor.stage.vfx.LoadVFX(effectData.res);
                                effs.Add(effectData.res, eff);
                                eff.transform.position = node.go.transform.position + effectData.position.ToVector().ToVector3();
                                eff.transform.rotation = Quaternion.Euler(node.go.transform.eulerAngles + effectData.eulerAngle.ToVector().ToVector3());
                                eff.transform.localScale = Vector3.one * effectData.scale * Config.int2Float;

                                first = true;
                            }

                            if (false == first && effectData.binding)
                            {
                                eff.transform.position = node.go.transform.position + effectData.position.ToVector().ToVector3();
                                eff.transform.rotation = Quaternion.Euler(node.go.transform.eulerAngles + effectData.eulerAngle.ToVector().ToVector3());
                                eff.transform.localScale = Vector3.one * effectData.scale * Config.int2Float;
                            }

                            eff.Play(GameDef.SP_DATA_LOGIC_FRAME_SCALE * GameDef.SP_DATA_TICK);

                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// 就绪检查或者初始化技能行为数据
        /// </summary>
        /// <param name="skill">技能 ID</param>
        private void ReadyOrInitialize(uint skill)
        {
            if (skillActionDatas.ContainsKey(skill)) return;
            var spdata = MessagePackSerializer.Deserialize<SkillPipelineData>(engine.gameres.location.LoadSkillDataSync(skill.ToString()));
            skillActionDatas.Add(skill, new List<SkillActionData>());
            for (int i = 0; i < spdata.actionIds.Length; i++)
            {
                var actionId = spdata.actionIds[i];
                switch (actionId)
                {
                    case SkillActionDef.ANIMATION:
                        skillActionDatas[skill].Add(MessagePackSerializer.Deserialize<AnimationActionData>(spdata.actionBytes[i]));
                        break;
                    case SkillActionDef.EFFECT:
                        skillActionDatas[skill].Add(MessagePackSerializer.Deserialize<EffectActionData>(spdata.actionBytes[i]));
                        break;
                }
            }
        }
    }
}
