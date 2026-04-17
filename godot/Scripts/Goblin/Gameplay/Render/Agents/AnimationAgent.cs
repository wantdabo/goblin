using Goblin.Common;
using Goblin.Common.GameRes;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Core;
using Godot;
using System.Text.Json;

namespace Goblin.Gameplay.Render.Agents
{
    /// <summary>
    /// 动画代理，使用 Godot AnimationPlayer 替代 Animancer
    /// </summary>
    public class AnimationAgent : Agent
    {
        private string cfgname;
        private AnimationConfig animcfg;
        private AnimationPlayer animplayer;
        private string preplayname;
        private string curplayname;
        private string playname;
        private float tarduration;
        private float mixduration;

        protected override void OnReady()
        {
            cfgname = null; animcfg = null; animplayer = null;
            preplayname = null; curplayname = null; playname = null;
            tarduration = 0; mixduration = 0;
            WatchRIL<RIL_FACADE_ANIMATION>(OnRILStateMachine);
        }

        protected override void OnReset()
        {
            cfgname = null; animcfg = null; animplayer = null;
            preplayname = null; curplayname = null; playname = null;
            tarduration = 0; mixduration = 0;
        }

        private void OnRILStateMachine(RIL_FACADE_ANIMATION ril) => RILConv2AnimData(ril);

        private void RILConv2AnimData(RIL_FACADE_ANIMATION ril)
        {
            if (false == world.rilbucket.SeekRIL(ril.actor, out RIL_FACADE_MODEL facademodel) || 0 >= facademodel.model) return;
            if (false == world.engine.cfg.location.ModelInfos.TryGetValue(facademodel.model, out var modelinfo)) return;

            if (string.IsNullOrEmpty(cfgname) || !modelinfo.Animation.Equals(cfgname))
            {
                cfgname = modelinfo.Animation;
                var bytes = world.engine.gameres.LoadRawFileSync(Location.animcfgpath + cfgname + ".json");
                if (bytes != null && bytes.Length > 0)
                    animcfg = JsonSerializer.Deserialize<AnimationConfig>(bytes);
            }

            var animname = ril.animname ?? animcfg?.GetAnimationName(ril.animstate);
            if (curplayname != animname) { preplayname = curplayname; curplayname = animname; }

            playname = curplayname;
            mixduration = 0;
            tarduration = ril.animelapsed * Config.Int2Float;

            var animinfo = animcfg?.GetAnimationMixInfo(animname);
            if (null == animinfo) return;
            mixduration = animinfo.mixduration;

            if (null == preplayname) return;
            var beforeInfo = animinfo.GetAnimationBeforeMixInfo(preplayname);
            if (null != beforeInfo && tarduration < beforeInfo.duration)
            {
                playname = beforeInfo.name;
                mixduration = beforeInfo.mixduration;
            }
        }

        protected override void OnFlash()
        {
            base.OnFlash();
            if (null == animplayer || string.IsNullOrEmpty(playname)) return;
            if (animplayer.HasAnimation(playname)) animplayer.Seek(tarduration, true);
        }

        protected override void OnChase(float tick, float timescale)
        {
            base.OnChase(tick, timescale);
            if (null == animplayer)
            {
                var model = world.GetAgent<ModelAgent>(actor);
                if (null == model?.node) return;
                animplayer = model.node.FindChild("AnimationPlayer", true, false) as AnimationPlayer;
            }
            if (null == animplayer || string.IsNullOrEmpty(playname)) return;
            if (!animplayer.HasAnimation(playname)) return;

            if (animplayer.CurrentAnimation != playname)
                animplayer.Play(playname, mixduration > 0 ? mixduration * (1f / timescale) : -1);

            animplayer.SpeedScale = 0;
            var newTime = Mathf.Clamp(animplayer.CurrentAnimationPosition + tick * timescale, 0, tarduration);
            animplayer.Seek(newTime, true);
        }
    }
}
