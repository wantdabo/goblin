using Goblin.Core;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Behaviors
{
    /// <summary>
    /// Node/节点
    /// </summary>
    public class Node : Behavior
    {
        /// <summary>
        /// Node 根
        /// </summary>
        private static GameObject nodes = new("NODES");
        /// <summary>
        /// GameObject
        /// </summary>
        public GameObject go { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            Debug.Log("OnCreate ===========================>");
            go = engine.pool.Get<GameObject>("NODE_GO_KEY");
            if (null == go) go = new();
            go.SetActive(true);
            go.name = actor.id.ToString();
            go.transform.SetParent(nodes.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (null == go) return;
            engine.pool.Set("NODE_GO_KEY", go);
            go.SetActive(false);
        }
    }
}
