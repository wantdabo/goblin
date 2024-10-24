﻿using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Goblin.Gameplay.Logic.Spatials;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    public class SpatialAction : SkillAction<SpatialActionData>
    {
        public override ushort id => SkillActionDef.SPATIAL;
        
        private Spatial spatial { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = pipeline.launcher.actor.GetBehavior<Spatial>();
        }

        protected override void OnExecute(SpatialActionData data, uint frame, FP tick)
        {
            if (null == spatial) return;

            var position = data.position.ToVector();
            var scale = data.scale * FP.EN3;

            var t = FP.One / (1 + data.eframe - data.sframe);
            spatial.position += spatial.rotation * (position * t);
            // TODO 支持角度变化
            spatial.scale += TSVector.one * scale * t;
        }
    }
}
