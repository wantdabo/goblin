using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.Sys.Common
{
    /// <summary>
    /// UI 特效控制器
    /// </summary>
    [DisallowMultipleComponent]
    public class UIEffectController : MonoBehaviour
    {
        /// <summary>
        /// 粒子系统集合
        /// </summary>
        [HideInInspector]
        public ParticleSystem[] pss;

        [HideInInspector]
        private Dictionary<ParticleSystem, int> cachePsSortingDict = new();

        /// <summary>
        /// 面片集合
        /// </summary>
        [HideInInspector]
        public MeshRenderer[] renders;

        [HideInInspector]
        private Dictionary<MeshRenderer, int> cacheRenderSortingDict = new();

        /// <summary>
        /// 动画集合
        /// </summary
        [HideInInspector]
        public Animator[] animators;

        /// <summary>
        /// 持续时间
        /// </summary>
        [HideInInspector]
        public float duration;

        /// <summary>
        /// 排序层级
        /// </summary>
        [HideInInspector]
        public string layerName;

        /// <summary>
        /// 排序顺序
        /// </summary>
        [HideInInspector]
        public int sorting;

        private void OnEnable()
        {
            cachePsSortingDict.Clear();
            foreach (var ps in pss) cachePsSortingDict.Add(ps, ps.GetComponentInChildren<Renderer>().sortingOrder);

            cacheRenderSortingDict.Clear();
            foreach (var render in renders) cacheRenderSortingDict.Add(render, render.sortingOrder);
        }

        private bool destroyed = false;

        private void OnDestroy()
        {
            destroyed = true;
        }

        /// <summary>
        /// 动态调整渲染层级
        /// </summary>
        public void AdjSorting()
        {
            foreach (var ps in pss)
            {
                var renderer = ps.GetComponentInChildren<Renderer>();
                renderer.sortingLayerName = layerName;
                cachePsSortingDict.TryGetValue(ps, out var order);
                renderer.sortingOrder = order + sorting;
            }

            foreach (var render in renders)
            {
                render.sortingLayerName = layerName;
                cacheRenderSortingDict.TryGetValue(render, out var order);
                render.sortingOrder = order + sorting;
            }
        }

        public void Stop(string stateName = "")
        {
            if (destroyed) return;
            transform.gameObject.SetActive(false);
            foreach (var anim in animators) if (anim.isActiveAndEnabled) anim.Play(stateName);
        }

        public void Play(string stateName)
        {
            if (destroyed) return;
            transform.gameObject.SetActive(true);
            foreach (var anim in animators) if (anim.gameObject.activeInHierarchy) anim.Play(stateName);
        }
    }
}
