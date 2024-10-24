using Goblin.Common;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Behaviors
{
    /// <summary>
    /// 模型变化事件
    /// </summary>
    public struct ModelChangedEvent : IEvent
    {
    }

    /// <summary>
    /// 模型
    /// </summary>
    public class Model : Behavior
    {
        /// <summary>
        /// 模型节池
        /// </summary>
        private static GameObject modelpool = new("MODELPOOL");
        static Model()
        {
            modelpool.SetActive(false);
        }
        
        /// <summary>
        /// 资源名
        /// </summary>
        public string res { get; private set; }
        /// <summary>
        /// 模型 GameObject
        /// </summary>
        public GameObject go { get; private set; }
        private Node node { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            node = actor.EnsureBehavior<Node>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (null != go) engine.pool.Set($"MODEL_GO_KEY_{res}", go, (g) => g.transform.SetParent(modelpool.transform));
        }
    
        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="res">资源名</param>
        public void Load(string res)
        {
            if (null != this.res && res.Equals(this.res)) return;
            if (null != go) engine.pool.Set($"MODEL_GO_KEY_{res}", go, (g) => g.transform.SetParent(modelpool.transform));
            this.res = res;

            go = engine.pool.Get<GameObject>($"MODEL_GO_KEY_{res}");
            if (null == go) go = engine.gameres.location.LoadModelSync(res);
            if (null == go) return;
            if (null == node || null == node.go)
            {
                engine.pool.Set($"MODEL_GO_KEY_{res}", go, (g) => g.transform.SetParent(modelpool.transform));
                return;
            }

            go.transform.SetParent(node.go.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            actor.eventor.Tell<ModelChangedEvent>();
        }
    }
}
