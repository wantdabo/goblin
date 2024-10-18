using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Effects;
using MessagePack;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SkillPipelineInfo : Resolver<RIL_SKILLPIPELINE_INFO>
    {
        public override ushort id => RILDef.SKILLPIPELINE_INFO;
        private Node node { set; get; }
        private AnimancerAnimation animation { get; set; }
        private Dictionary<uint, uint> skillLengths { get; set; } = new();
        private Dictionary<uint, List<SkillActionData>> skillActionDatas { get; set; } = new();
        
        protected override void OnAwake(uint frame, RIL_SKILLPIPELINE_INFO ril)
        {
            node = actor.EnsureBehavior<Node>();
            animation = actor.EnsureBehavior<AnimancerAnimation>();
        }

        private Dictionary<uint, VFXController> effdict = new();

        protected override void OnResolve(uint frame, RIL_SKILLPIPELINE_INFO ril)
        {
            ReadyOrInitialize(ril.skillid);
            var t = ril.frame / (float)ril.length;
            var length = skillLengths[ril.skillid];
            if (skillActionDatas.TryGetValue(ril.skillid, out var actions))
            {
                foreach (var action in actions)
                {
                    var st = action.sframe / (float)length;
                    var et = action.eframe / (float)length;
                    var between = (t >= st && t <= et + GameDef.SP_DATA_TICK);
                    switch (action.id)
                    {
                        case SkillActionDef.ANIMATION:
                            if (false == between) break;
                            var animationData = (AnimationActionData)action;
                            animation.Play(animationData.name, t / et);
                            break;
                        case SkillActionDef.EFFECT:
                            // TODO 还需要优化，非正式代码
                            var key = ril.id + (uint)action.id + action.sframe + action.eframe;
                            if (false == between)
                            {
                                if (effdict.TryGetValue(key, out var eff))
                                {
                                    eff.Stop();
                                    actor.stage.vfx.UnloadVFX(eff);
                                    effdict.Remove(key);
                                }
                                break;
                            }

                            var effectData = (EffectActionData)action;
                            if (effectData.positionBinding)
                            {
                            }
                            else
                            {
                                effdict.TryGetValue(key, out var eff);
                                if (null == eff)
                                {
                                    eff = actor.stage.vfx.LoadVFX(effectData.res);
                                    eff.transform.position = node.go.transform.position + effectData.position.ToVector().ToVector3();
                                    eff.transform.rotation = Quaternion.Euler(node.go.transform.eulerAngles + effectData.eulerAngle.ToVector().ToVector3());
                                    eff.transform.localScale = Vector3.one * effectData.scale * Config.int2Float;
                                    eff.Play();
                                    effdict.Add(key, eff);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void ReadyOrInitialize(uint skill)
        {
            if (skillActionDatas.ContainsKey(skill)) return;
            var spdata = MessagePackSerializer.Deserialize<SkillPipelineData>(engine.gameres.location.LoadSkillDataSync(skill.ToString()));
            skillLengths.Add(skill, spdata.length);
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
