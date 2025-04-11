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
        private static GameObject nodes = new("NODES");
        /// <summary>
        /// GameObject
        /// </summary>
        public GameObject go { get; private set; }
        
        protected override void OnReady()
        {
            go = ObjectCache.Get<GameObject>("NODE_GO_KEY");
            go.SetActive(true);
            go.name = actor.ToString();
            go.transform.SetParent(nodes.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            
            // TODO Test
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale *= 0.5f;
            cube.transform.SetParent(go.transform);
        }

        protected override void OnReset()
        {
            if (null == go) return;
            go.SetActive(false);
            world.engine.pool.Set(go, "NODE_GO_KEY");
        }
    }
}