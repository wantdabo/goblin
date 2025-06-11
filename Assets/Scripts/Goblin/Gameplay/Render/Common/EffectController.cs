using UnityEngine;

namespace Goblin.Gameplay.Render.Common
{
    /// <summary>
    /// 特效控制器
    /// </summary>
    [DisallowMultipleComponent]
    public class EffectController : MonoBehaviour
    {
        /// <summary>
        /// 动画状态机集合
        /// </summary>
        [HideInInspector] public Animator[] animators;
        /// <summary>
        /// 粒子系统集合
        /// </summary>
        [HideInInspector] public ParticleSystem[] pss;

        /// <summary>
        /// 特效采样
        /// </summary>
        /// <param name="time">时间</param>
        public void Simulate(float time)
        {
            // 粒子系统采样
            foreach (var ps in pss) if (ps.gameObject.activeInHierarchy) ps.Simulate(time, true, true);

            // 动画状态机采样
            foreach (var animator in animators)
            {
                if (false == animator.gameObject.activeInHierarchy) continue;

                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                var duration = stateInfo.length;
                var normalizedTime = time / duration;

                animator.Play(stateInfo.fullPathHash, 0, normalizedTime);
                animator.Update(0);
            }
        }
    }
}