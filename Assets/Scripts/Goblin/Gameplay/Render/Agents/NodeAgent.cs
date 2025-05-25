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
            go = ObjectPool.Ensure<GameObject>("NODE_GO_KEY");
            if (null == go)
            {
                go = new GameObject();
                go.SetActive(false);
            }
            go.name = actor.ToString();
            go.transform.SetParent(root.transform);

            WatchRIL<RIL_SPATIAL>(OnRILSpatial);
            WatchRIL<RIL_MOTION>(OnRILMotion);
        }

        protected override void OnReset()
        {
            if (null == go) return;

            go.SetActive(false);
            ObjectPool.Set(go, "NODE_GO_KEY");
        }

        private void OnRILSpatial(RIL_SPATIAL ril)
        {
            ShouldBeChange();
        }
        
        private void OnRILMotion(RIL_MOTION ril)
        {
            ShouldBeChange();
        }

        /// <summary>
        /// 是否需要改变状态
        /// </summary>
        private void ShouldBeChange()
        {
            if (false == world.rilbucket.SeekRIL<RIL_MOTION>(actor, out var ril))
            {
                ChangeStatus(ChaseStatus.Chasing);
                return;
            }
            
            switch (ril.motion)
            {
                case MOTION_DEFINE.MOTION:
                    ChangeStatus(ChaseStatus.Chasing);
                    break;
                case MOTION_DEFINE.POSITION:
                    Flash();
                    break;
            }   
        }

        protected override bool OnArrived()
        {
            if (false == world.rilbucket.SeekRIL<RIL_SPATIAL>(actor, out var ril)) return true;
            return go.transform.position == ril.position.ToVector3() &&
                   go.transform.rotation.eulerAngles == ril.euler.ToVector3() &&
                   go.transform.localScale == Vector3.one * ril.scale.AsFloat();
        }
        
        protected override void OnFlash()
        {
            base.OnFlash();
            if (false == world.rilbucket.SeekRIL<RIL_SPATIAL>(actor, out var ril)) return;
            go.transform.position = ril.position.ToVector3();
            go.transform.rotation = Quaternion.Euler(ril.euler.ToVector3());
            go.transform.localScale = Vector3.one * ril.scale.AsFloat();
            go.SetActive(true);
        }
    }
}