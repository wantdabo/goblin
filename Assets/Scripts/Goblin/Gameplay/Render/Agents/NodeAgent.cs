using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Agents
{
    /// <summary>
    /// 节点代理
    /// </summary>
    public class NodeAgent : Agent
    {
        /// <summary>
        /// Node 根
        /// </summary>
        private static GameObject root = new("Node");
        static NodeAgent()
        {
            root.transform.SetParent(GameObject.Find("Gameplay").transform, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// GameObject
        /// </summary>
        public GameObject go { get; private set; }
        public Vector3 targetPosition { get; set; }
        public Vector3 targetEuler { get; set; }
        public Vector3 targetScale { get; set; }
        
        protected override void OnReady()
        {
            go = ObjectCache.Get<GameObject>("NODE_GO_KEY");
            if (null == go) go = new GameObject();
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
            ObjectCache.Set(go, "NODE_GO_KEY");
            
            targetPosition = Vector3.zero;
            targetEuler = Vector3.zero;
            targetScale = Vector3.one;
        }

        protected override void OnArrive()
        {
            base.OnArrive();
            go.transform.position = targetPosition;
            go.transform.rotation = Quaternion.Euler(targetEuler);
            go.transform.localScale = targetScale;
        }

        protected override void OnChase(float tick, float timescale)
        {
            base.OnChase(tick, timescale);
            var t = tick / GAME_DEFINE.LOGIC_TICK.AsFloat();
            go.transform.position = Vector3.Lerp(go.transform.position, targetPosition, t);
            
            var lastrot = Quaternion.Euler(go.transform.eulerAngles);
            var targetrot = Quaternion.Euler(targetEuler);
            go.transform.rotation = Quaternion.Slerp(lastrot, targetrot, t);
        }
    }
}