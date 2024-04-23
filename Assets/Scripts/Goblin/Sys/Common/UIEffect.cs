using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.Sys.Common
{
    /// <summary>
    /// UI 特效
    /// </summary>
    public class UIEffect : Comp
    {
        private UIEffectController uiec;

        public UIEffectController UIEC { get => uiec; }
        /// <summary>
        /// 加载特效
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="resName">特效名</param>
        public void Load(GameObject node, string resName)
        {
            var effGo = engine.gameres.location.LoadUIEffectSync(resName);
            uiec = effGo.GetComponent<UIEffectController>();
            uiec.transform.SetParent(node.transform, false);
            Stop();
        }

        /// <summary>
        /// 特效层级排序
        /// </summary>
        /// <param name="layerName">层级名</param>
        /// <param name="sorting">层级编号</param>
        public void Sorting(string layerName, int sorting)
        {
            uiec.layerName = layerName;
            uiec.sorting = sorting;
            uiec.AdjSorting();
        }

        /// <summary>
        /// 停止播放特效
        /// </summary>
        public void Stop()
        {
            engine.ticker.StopTimer(delayTimingId);
            uiec.Stop();
        }

        private uint delayTimingId;
        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="stateName">状态机动画名</param>
        public void Play(string stateName = "")
        {
            if (uiec.duration > 0)
            {
                engine.ticker.Timing((t) =>
                {
                    Stop();
                }, uiec.duration, 1);
            }
            delayTimingId = engine.ticker.Timing((t) => { uiec.Play(stateName); }, 0.05f, 1);
        }
    }
}
