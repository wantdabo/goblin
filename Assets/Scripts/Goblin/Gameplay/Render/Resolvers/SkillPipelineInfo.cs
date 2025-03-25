using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Common.SkillDatas.Common;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Effects;
using MessagePack;
using System.Collections.Generic;
using Goblin.Gameplay.Common.Extensions;
using UnityEngine;
using Animation = Goblin.Gameplay.Render.Behaviors.Common.Animation;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 技能管线解释器
    /// </summary>
    public class SkillPipelineInfo : Resolver<RIL_SKILL_PIPELINE_INFO>
    {
        private Node node { set; get; }

        private Animation animation { get; set; }

        /// <summary>
        /// 技能行为数据列表
        /// </summary>
        private Dictionary<uint, List<SkillActionData>> skillActionDatas { get; set; } = new();

        /// <summary>
        /// 技能特效进行中列表
        /// </summary>
        private Dictionary<uint, Dictionary<string, VFXController>> effdict = new();

        protected override void OnAwake(uint frame, RIL_SKILL_PIPELINE_INFO ril)
        {
            node = actor.EnsureBehavior<Node>();
            animation = actor.EnsureBehavior<AnimancerAnimation>();
        }

        protected override void OnResolve(uint frame, RIL_SKILL_PIPELINE_INFO ril)
        {
            if (SKILL_PIPELINE_STATE_DEFINE.END == ril.state || SKILL_PIPELINE_STATE_DEFINE.NONE == ril.state)
            {
                if (effdict.TryGetValue(ril.skillid, out var effs))
                {
                    foreach (var eff in effs.Values) eff.Stop();
                    effs.Clear();
                }
                return;
            }

            ReadyOrInitialize(ril.skillid);
            var f = Mathf.Floor((ril.frame - 1) * GAME_DEFINE.SP_DATA_LOGIC_FRAME_SCALE);
            if (skillActionDatas.TryGetValue(ril.skillid, out var datas))
            {
                foreach (var data in datas)
                {
                    var between = (f >= data.sframe && f <= data.eframe);
                    switch (data.id)
                    {
                        case SKILL_ACTION_DEFINE.ANIMATION:
                            if (false == between) break;
                            var animationData = (AnimationData)data;
                            animation.Play(animationData.name, f / data.eframe);
                            break;
                        case SKILL_ACTION_DEFINE.EFFECT:
                            if (false == effdict.TryGetValue(ril.skillid, out var effs))
                            {
                                effs = new();
                                effdict.Add(ril.skillid, effs);
                            }

                            EffectData effectData = (EffectData)data;
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
                                eff.transform.position = node.go.transform.position + effectData.position.ToFPVector3().ToVector3();
                                eff.transform.rotation = Quaternion.Euler(node.go.transform.eulerAngles + effectData.eulerAngles.ToFPVector3().ToVector3());
                                eff.transform.localScale = Vector3.one * effectData.scale * Config.int2Float;

                                first = true;
                            }

                            if (false == first && effectData.binding)
                            {
                                eff.transform.position = node.go.transform.position + effectData.position.ToFPVector3().ToVector3();
                                eff.transform.rotation = Quaternion.Euler(node.go.transform.eulerAngles + effectData.eulerAngles.ToFPVector3().ToVector3());
                                eff.transform.localScale = Vector3.one * effectData.scale * Config.int2Float;
                            }

                            eff.Play(GAME_DEFINE.SP_DATA_LOGIC_FRAME_SCALE * GAME_DEFINE.SP_DATA_TICK);

                            break;
                        case SKILL_ACTION_DEFINE.SOUND:
                            // TODO 后面需要优化支持音效加减速/配合顿帧、游戏加速等等
                            if ((uint)f != data.sframe) break;
                            var soundData = (SoundData)data;
                            engine.sound.Load(soundData.res).Play();
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
                    case SKILL_ACTION_DEFINE.ANIMATION:
                        skillActionDatas[skill].Add(MessagePackSerializer.Deserialize<AnimationData>(spdata.actionBytes[i]));
                        break;
                    case SKILL_ACTION_DEFINE.EFFECT:
                        skillActionDatas[skill].Add(MessagePackSerializer.Deserialize<EffectData>(spdata.actionBytes[i]));
                        break;
                    case SKILL_ACTION_DEFINE.SOUND:
                        skillActionDatas[skill].Add(MessagePackSerializer.Deserialize<SoundData>(spdata.actionBytes[i]));
                        break;
                }
            }
        }
    }
}
