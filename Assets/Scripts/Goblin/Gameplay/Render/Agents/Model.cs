using Goblin.Common;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Agents
{
    /// <summary>
    /// 模型变化事件
    /// </summary>
    public struct ModelChangedEvent : IEvent
    {
    }
    
    /// <summary>
    /// 模型代理
    /// </summary>
    public class Model : Agent
    {
        /// <summary>
        /// 模型节池
        /// </summary>
        private static GameObject modelpool = new("ModelPool");
        static Model()
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
            this.id = 0;
            this.res = null;
            this.go = null;
        }

        protected override void OnReset()
        {
            if (null != go)
            {
                world.engine.pool.Set(go, $"MODEL_GO_KEY_{this.res}");
                go.transform.SetParent(modelpool.transform, false);
            }
            this.id = 0;
            this.res = null;
        }
    
        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="id">模型 ID</param>
        public void Load(int id)
        {
            if (this.id == id) return;
            this.id = id;
            var modelinfo = world.engine.cfg.location.ModelInfos.Get(this.id);
            this.res = modelinfo.Res;
            
            go = world.engine.pool.Get<GameObject>($"MODEL_GO_KEY_{this.res}");
            if (null == go) go = world.engine.gameres.location.LoadModelSync(this.res);
            
            var node = world.EnsureAgent<Node>(actor);
            go.transform.SetParent(node.go.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }
    }
}