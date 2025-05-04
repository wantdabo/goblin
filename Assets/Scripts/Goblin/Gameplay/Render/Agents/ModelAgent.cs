using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.States;
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
        public int id { get; private set; } = 0;
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
            this.id = 0;
            this.res = null;
            this.go = null;
        }

        protected override void OnReset()
        {
            RecycleModel();
            this.id = 0;
            this.res = null;
            this.go = null;
        }
        
        /// <summary>
        /// 回收模型 Go
        /// </summary>
        private void RecycleModel()
        {
            if (null != go)
            {
                world.engine.pool.Set(go, $"MODEL_GO_KEY_{this.res}");
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
            if (false == world.statebucket.SeekState<TagState>(actor, out var state)) return;
            if (false == state.tags.TryGetValue(TAG_DEFINE.MODEL, out var id))
            {
                RecycleModel();
                return;
            }
            if (this.id == id) return;
            
            RecycleModel();
            this.id = id;
            var modelinfo = world.engine.cfg.location.ModelInfos.Get(this.id);
            this.res = modelinfo.Res;
            
            go = world.engine.pool.Get<GameObject>($"MODEL_GO_KEY_{this.res}");
            if (null == go) go = world.engine.gameres.location.LoadModelSync(this.res);
            var node = world.EnsureAgent<NodeAgent>(actor);
            go.transform.SetParent(node.go.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }
    }
}