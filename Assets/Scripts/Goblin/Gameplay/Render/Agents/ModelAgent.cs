using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Agents
{
    /// <summary>
    /// 模型代理
    /// </summary>
    public class ModelAgent : Agent
    {
        /// <summary>
        /// 模型节池
        /// </summary>
        private static GameObject modelpool = new("ModelPool");
        static ModelAgent()
        {
            modelpool.transform.SetParent(GameObject.Find("Gameplay").transform, false);
            modelpool.transform.localPosition = Vector3.zero;
            modelpool.transform.localScale = Vector3.one;
            modelpool.SetActive(false);
        }
        
        /// <summary>
        /// 模型 ID
        /// </summary>
        public int model { get; private set; } = 0;
        /// <summary>
        /// 资源名
        /// </summary>
        public string res { get; private set; }
        /// <summary>
        /// 模型 GameObject
        /// </summary>
        public GameObject go { get; private set; }
        
        protected override void OnReady()
        {
            RecycleModel();
            model = 0;
            res = null;
            go = null;
        }

        protected override void OnReset()
        {
            RecycleModel();
            model = 0;
            res = null;
            go = null;
        }
        
        /// <summary>
        /// 回收模型 Go
        /// </summary>
        private void RecycleModel()
        {
            if (null != go)
            {
                ObjectPool.Set(go, res);
                go.transform.SetParent(modelpool.transform, false);
            }
        }

        protected override void OnChase(float tick, float timescale)
        {
            base.OnChase(tick, timescale);
            Load();
        }

        /// <summary>
        /// 加载模型
        /// </summary>
        public void Load()
        {
            if (false == world.rilbucket.SeekRIL(actor, out RIL_FACADE_MODEL facademodel) || 0 >= facademodel.model)
            {
                RecycleModel();
                return;
            }
            if (model == facademodel.model) return;
            
            var node = world.EnsureAgent<NodeAgent>(actor);
            if (null == node || null == node.go) return;
            
            RecycleModel();
            model = facademodel.model;
            if (false == world.engine.cfg.location.ModelInfos.TryGetValue(facademodel.model, out var modelinfo)) return;
            res = modelinfo.Res;
            go = ObjectPool.Get<GameObject>(res);
            if (null == go) go = world.engine.gameres.location.LoadModelSync(res);
            go.transform.SetParent(node.go.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }
    }
}