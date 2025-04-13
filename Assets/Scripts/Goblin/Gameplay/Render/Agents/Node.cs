using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Agents
{
    public class Node : Agent
    {
        /// <summary>
        /// Node 根
        /// </summary>
        private static GameObject root = new("Node");
        static Node()
        {
            root.transform.SetParent(GameObject.Find("Gameplay").transform, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// GameObject
        /// </summary>
        public GameObject go { get; private set; }
        
        protected override void OnReady()
        {
            go = ObjectCache.Get<GameObject>("NODE_GO_KEY");
            go.SetActive(true);
            go.name = actor.ToString();
            go.transform.SetParent(root.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }

        protected override void OnReset()
        {
            if (null == go) return;
            go.SetActive(false);
            world.engine.pool.Set(go, "NODE_GO_KEY");
        }
    }
}