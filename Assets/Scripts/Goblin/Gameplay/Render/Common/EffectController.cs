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
    }
}