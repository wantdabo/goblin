using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 外观行为
    /// </summary>
    public class Facade : Behavior<FacadeInfo>
    {
        /// <summary>
        /// 设置模型
        /// </summary>
        /// <param name="model">模型 ID</param>
        public void SetModel(int model)
        {
            info.model = model;
        }

        /// <summary>
        /// 设置动画状态
        /// </summary>
        /// <param name="state">动画状态</param>
        public void SetAnimation(byte state)
        {
            info.animstate = state;
            info.animelapsed = 0;
        }
        
        /// <summary>
        /// 设置动画名称
        /// </summary>
        /// <param name="animname">动画名称</param>
        public void SetAnimation(string animname)
        {
            info.animname = animname;
            info.animelapsed = 0;
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="effect">特效</param>
        public uint CreateEffect(EffectInfo effect)
        {
            var increment = info.effectincrement++;
            effect.id = increment;
            effect.elapsed = 0;
            info.effects.Add(effect.id);
            info.effectdict.Add(effect.id, effect);
            
            return increment;
        }

        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            info.animelapsed += tick;

            // 移除过期的特效
            foreach (var rmveffect in info.rmveffects)
            {
                info.effects.Remove(rmveffect);
                info.effectdict.Remove(rmveffect);
            }
            info.rmveffects.Clear();

            // 更新特效时间流逝
            foreach (var id in info.effects)
            {
                if (false == info.effectdict.TryGetValue(id, out var effect)) continue;
                effect.elapsed += tick;
                info.effectdict.Remove(id);
                info.effectdict.Add(id, effect);
                if (effect.elapsed >= effect.duration) info.rmveffects.Add(id);
            }
        }
    }
}