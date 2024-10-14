using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using MessagePack;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SkillPipelineInfo : Resolver<RIL_SKILLPIPELINE_INFO>
    {
        public override ushort id => RILDef.SKILLPIPELINE_INFO;
        private AnimancerAnimation animation { get; set; }
        private Dictionary<uint, uint> skillLengths { get; set; } = new();
        private Dictionary<uint, List<SkillActionData>> skillActionDatas { get; set; } = new();

        protected override void OnAwake(uint frame, RIL_SKILLPIPELINE_INFO ril)
        {
            animation = actor.EnsureBehavior<AnimancerAnimation>();
        }

        protected override void OnResolve(uint frame, RIL_SKILLPIPELINE_INFO ril)
        {
            ReadyOrInitialize(ril.skillid);
            var t = ril.frame / (float)ril.length;
            var length = skillLengths[ril.skillid];
            if (skillActionDatas.TryGetValue(ril.skillid, out var actions))
            {
                foreach (var action in actions)
                {
                    switch (action.id)
                    {
                        case SkillActionDef.ANIMATION:
                            var animationData = (AnimationActionData)action;
                            var st = animationData.sframe / (float)length;
                            var et = animationData.eframe / (float)length;
                            if (t >= st && t <= et)
                            {
                                animation.Play(animationData.name, t - st, et);
                            }
                            break;
                    }
                }
            }
        }

        private void ReadyOrInitialize(uint sp)
        {
            if (skillActionDatas.ContainsKey(sp)) return;
            var spdata = MessagePackSerializer.Deserialize<SkillPipelineData>(engine.gameres.location.LoadSkillDataSync(sp.ToString()));
            skillLengths.Add(sp, spdata.length);
            skillActionDatas.Add(sp, new List<SkillActionData>());
            for (int i = 0; i < spdata.actionIds.Length; i++)
            {
                var actionId = spdata.actionIds[i];
                switch (actionId)
                {
                    case SkillActionDef.ANIMATION:
                        skillActionDatas[sp].Add(MessagePackSerializer.Deserialize<AnimationActionData>(spdata.actionBytes[i]));
                        break;
                }
            }
        }
    }
}
