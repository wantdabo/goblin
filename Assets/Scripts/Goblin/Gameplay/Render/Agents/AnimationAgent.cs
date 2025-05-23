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
        /// 动画名称
        /// </summary>
        private string animname { get; set; }
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
            
            WatchRIL<RIL_STATE_MACHINE>((ril) =>
            {
                RILConv2AnimData(ril);
            });
        }

        protected override void OnReset()
        {
            animancer = null;
            animstate = null;
        }

        /// <summary>
        /// 状态机状态转换为动画数据
        /// </summary>
        /// <param name="ril">状态机状态</param>
        private void RILConv2AnimData(RIL_STATE_MACHINE ril)
        {
            if (false == world.rilbucket.SeekRIL(ril.actor, out RIL_FACADE facade) || 0 >= facade.model) return;
            var modelinfo = world.engine.cfg.location.ModelInfos.Get(facade.model);
            
            if (string.IsNullOrEmpty(cfgname) || false == modelinfo.Animation.Equals(cfgname))
            {
                cfgname = modelinfo.Animation;
                if (null != animcfg) GameObject.Destroy(animcfg);
                animcfg = world.engine.gameres.location.LoadModelAnimationConfigSync(cfgname);
            }
                
            var animinfo = animcfg.GetAnimationInfo(ril.current);
            if (null == animinfo) return;
                
            animname = animinfo.name;
            mixduration = animinfo.mixduration;
            tarduration = ril.elapsed * Config.Int2Float;

            var beforeAnimInfo = animinfo.GetMixAnimationInfo(ril.last);
            if (null != beforeAnimInfo && tarduration < beforeAnimInfo.duration)
            {
                animname = beforeAnimInfo.name;
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
            animstate = animancer.TryPlay(animname, mixduration * (1 / timescale));
            if (null == animstate) return;
            animstate.Speed = 0;
            animstate.Time = Mathf.Clamp(animstate.Time + tick * timescale, 0, tarduration);
        }
    }
}