using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Goblin.Gameplay.Render.Effects
{
    public class VisualEffect : Comp
    {
        /// <summary>
        /// 特效对象池
        /// </summary>
        private static GameObject vfxpool = new("VFXPOOL");

        private static GameObject vfxstage = new("VFXSTAGES");

        public Stage stage { get; set; }

        private List<VFXController> vfxcs = new();

        static VisualEffect()
        {
            vfxpool.SetActive(false);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            stage.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stage.ticker.eventor.UnListen<TickEvent>(OnTick);
            for (int i = vfxcs.Count - 1; i >= 0; i--) UnloadVFX(vfxcs[i]);
            vfxcs.Clear();
        }

        /// <summary>
        /// 卸载特效
        /// </summary>
        /// <param name="vfxc">特效</param>
        /// <exception cref="Exception">卸载的对象不能为空</exception>
        public void UnloadVFX(VFXController vfxc)
        {
            if (null == vfxc) throw new Exception("vfx can'count be null.");
            vfxc.vfx = null;
            vfxcs.Remove(vfxc);
            engine.pool.Set("VFX_CONTROLLER_KEY_" + vfxc.name, vfxc, (v) =>
            {
                v.transform.SetParent(vfxpool.transform);
                v.gameObject.SetActive(false);
            });
        }

        /// <summary>
        /// 加载特效
        /// </summary>
        /// <param name="name">特效名</param>
        /// <returns>特效</returns>
        public VFXController LoadVFX(string name)
        {
            var vfxc = engine.pool.Get<VFXController>("VFX_CONTROLLER_KEY_" + name);
            if (null == vfxc)
            {
                var go = engine.gameres.location.LoadEffectSync(name);
                vfxc = go.GetComponent<VFXController>();
                vfxc.gameObject.SetActive(false);
                vfxc.name = name;
            }
            vfxc.transform.SetParent(vfxstage.transform, false);
            vfxc.vfx = this;
            vfxc.transform.position = Vector3.zero;
            vfxc.transform.localScale = Vector3.one;
            vfxcs.Add(vfxc);

            return vfxc;
        }

        /// <summary>
        /// 加载特效
        /// </summary>
        /// <param name="name">特效名</param>
        /// <param name="gameObject">挂点/GameObject</param>
        /// <returns>特效</returns>
        public VFXController LoadVFX(string name, GameObject gameObject)
        {
            var eff = LoadVFX(name);
            eff.transform.SetParent(gameObject.transform, false);

            return eff;
        }

        /// <summary>
        /// 加载特效
        /// </summary>
        /// <param name="name">特效名</param>
        /// <param name="node">Node</param>
        /// <returns>特效</returns>
        public VFXController LoadVFX(string name, Node node)
        {
            return LoadVFX(name, node.go);
        }

        private void OnTick(TickEvent e)
        {
            foreach (var vfxc in vfxcs)
            {
                if (false == vfxc.playing) continue;
                vfxc.OnTick(e.tick);
            }
        }
    }
}
