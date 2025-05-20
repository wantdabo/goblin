using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Common.Extensions;
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
        
        protected override void OnReady()
        {
            go = ObjectCache.Get<GameObject>("NODE_GO_KEY");
            if (null == go) go = new GameObject();
            go.SetActive(true);
            go.name = actor.ToString();
            go.transform.SetParent(root.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            WatchRIL<RIL_SPATIAL>((ril) => ChangeStatus(ChaseStatus.Chasing));
        }

        protected override void OnReset()
        {
            if (null == go) return;

            go.SetActive(false);
            ObjectCache.Set(go, "NODE_GO_KEY");
        }
        
        protected override bool OnArrived()
        {
            if (false == world.rilbucket.SeekRIL<RIL_SPATIAL>(actor, out var ril)) return true;
            return go.transform.position == ril.position.ToVector3() &&
                   go.transform.rotation.eulerAngles == ril.euler.ToVector3() &&
                   go.transform.localScale == ril.scale.ToVector3();
        }
        
        protected override void OnFlash()
        {
            base.OnFlash();
            if (false == world.rilbucket.SeekRIL<RIL_SPATIAL>(actor, out var ril)) return;
            go.transform.position = ril.position.ToVector3();
            go.transform.rotation = Quaternion.Euler(ril.euler.ToVector3());
            go.transform.localScale = ril.scale.ToVector3();
        }
    }
}