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
        public Vector3 targetPosition { get; set; }
        public Vector3 targetEuler { get; set; }
        public Vector3 targetScale { get; set; }
        
        private Vector3 lastPosition { get; set; }
        private Vector3 lastEuler { get; set; }
        private float interpElapsed { get; set; }
        
        protected override void OnReady()
        {
            world.ticker.eventor.Listen<TickEvent>(OnTick);

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
            world.ticker.eventor.UnListen<TickEvent>(OnTick);

            go.SetActive(false);
            world.engine.pool.Set(go, "NODE_GO_KEY");
            
            targetPosition = Vector3.zero;
            targetEuler = Vector3.zero;
            targetScale = Vector3.one;
            
            lastPosition = Vector3.zero;
            lastEuler = Vector3.zero;
            interpElapsed = 0;
        }

        protected override void OnArrive()
        {
            base.OnArrive();
            go.transform.position = targetPosition;
            go.transform.rotation = Quaternion.Euler(targetEuler);
            go.transform.localScale = targetScale;
        }

        protected override void OnChase()
        {
            base.OnChase();
            lastPosition = go.transform.position;
            lastEuler = go.transform.eulerAngles;
            interpElapsed = 0f;
        }

        private void OnTick(TickEvent e)
        {
            interpElapsed += e.tick;

            var t = interpElapsed / GAME_DEFINE.LOGIC_TICK.AsFloat();
            go.transform.position = Vector3.Lerp(lastPosition, targetPosition, t);
            
            var lastrot = Quaternion.Euler(lastEuler);
            var targetrot = Quaternion.Euler(targetEuler);
            go.transform.rotation = Quaternion.Slerp(lastrot, targetrot, t);
        }
    }
}