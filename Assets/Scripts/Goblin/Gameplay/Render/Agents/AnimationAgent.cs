using Animancer;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;
using UnityEngine;

namespace Goblin.Gameplay.Render.Agents
{
    /// <summary>
    /// 动画代理
    /// </summary>
    public class AnimationAgent : Agent
    {
        /// <summary>
        /// 动画配置名称
        /// </summary>
        private string cfgname { get; set; } = default;
        /// <summary>
        /// 动画配置
        /// </summary>
        private AnimationConfig animcfg { get; set; } = default;
        /// <summary>
        /// Animancer 组件
        /// </summary>
        private AnimancerComponent animancer { get; set; }
        /// <summary>
        /// Animancer 播放状态
        /// </summary>
        private AnimancerState animstate { get; set; }
        /// <summary>
        /// 上一个动画名称
        /// </summary>
        private string preplayename { get; set; }
        /// <summary>
        /// 当前播放动画名称
        /// </summary>
        private string curplayname { get; set; }
        /// <summary>
        /// 动画名称
        /// </summary>
        private string playname { get; set; }
        /// <summary>
        /// 动画持续时间
        /// </summary>
        private float tarduration { get; set; }
        /// <summary>
        /// 混合持续时间
        /// </summary>
        private float mixduration { get; set; }
        
        protected override void OnReady()
        {
            animancer = null;
            animstate = null;
            preplayename = null;
            curplayname = null;
            playname = null;
            tarduration = 0;
            mixduration = 0;
            
            WatchRIL<RIL_FACADE>(OnRILStateMachine);
        }

        protected override void OnReset()
        {
            animancer = null;
            animstate = null;
            preplayename = null;
            curplayname = null;
            playname = null;
            tarduration = 0;
            mixduration = 0;
        }
        
        private void OnRILStateMachine(RIL_FACADE ril)
        {
            RILConv2AnimData(ril);
        }

        /// <summary>
        /// 状态机状态转换为动画数据
        /// </summary>
        /// <param name="ril">状态机状态</param>
        private void RILConv2AnimData(RIL_FACADE ril)
        {
            if (0 >= ril.model) return;
            if (false == world.engine.cfg.location.ModelInfos.TryGetValue(ril.model, out var modelinfo)) return;
            
            if (string.IsNullOrEmpty(cfgname) || false == modelinfo.Animation.Equals(cfgname))
            {
                cfgname = modelinfo.Animation;
                if (null != animcfg) GameObject.Destroy(animcfg);
                animcfg = world.engine.gameres.location.LoadAnimationConfigSync(cfgname);
            }

            var animname = ril.animname;
            if (null == animname) animname = animcfg.GetAnimationName(ril.animstate);
            
            if (curplayname != animname)
            {
                preplayename = curplayname;
                curplayname = animname;
            }
            
            playname = curplayname;
            mixduration = 0;
            tarduration = ril.animelapsed * Config.Int2Float;
            
            var animinfo = animcfg.GetAnimationMixInfo(animname);
            if (null == animinfo) return;
            mixduration = animinfo.mixduration;

            if (null == preplayename) return;
            var beforeAnimInfo = animinfo.GetAnimationBeforeMixInfo(preplayename);
            if (null != beforeAnimInfo && tarduration < beforeAnimInfo.duration)
            {
                playname = beforeAnimInfo.name;
                mixduration = beforeAnimInfo.mixduration;
            }
        }

        protected override void OnFlash()
        {
            base.OnFlash();
            if (null == animstate) return;
            animstate.Time = tarduration;
        }

        protected override void OnChase(float tick, float timescale)
        {
            base.OnChase(tick, timescale);
            if (null == animancer)
            {
                var model = world.GetAgent<ModelAgent>(actor);
                if (null == model || null == model.go) return;
                animancer = model.go.GetComponent<AnimancerComponent>();
            }

            if (null == animancer) return;
            animstate = animancer.TryPlay(playname, mixduration * (1 / timescale));
            if (null == animstate) return;
            animstate.Speed = 0;
            animstate.Time = Mathf.Clamp(animstate.Time + tick * timescale, 0, tarduration);
        }
    }
}