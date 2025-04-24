using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.States;
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

            CareState<SpatialState>((state) => ChangeStatus(ChaseStatus.Chasing));
        }

        protected override void OnReset()
        {
            if (null == go) return;

            go.SetActive(false);
            ObjectCache.Set(go, "NODE_GO_KEY");
        }

        protected override bool OnArrived()
        {
            if (false == world.statebucket.SeekState<SpatialState>(actor, out var state)) return true;
            return go.transform.position == state.position && go.transform.rotation.eulerAngles == state.euler && go.transform.localScale == state.scale;
        }

        protected override void OnFlash()
        {
            base.OnFlash();
            if (false == world.statebucket.SeekState<SpatialState>(actor, out var state)) return;
            go.transform.position = state.position;
            go.transform.rotation = Quaternion.Euler(state.euler);
            go.transform.localScale = state.scale;
        }

        protected override void OnChase(float tick, float timescale)
        {
            base.OnChase(tick, timescale);
            if (false == world.statebucket.SeekState<SpatialState>(actor, out var state)) return;
            
            var t = tick / GAME_DEFINE.LOGIC_TICK.AsFloat();
            go.transform.position = Vector3.Lerp(go.transform.position, state.position, t);
            go.transform.rotation = Quaternion.Slerp(Quaternion.Euler(go.transform.eulerAngles), Quaternion.Euler(state.euler), t);
            go.transform.localScale = state.scale;
        }
    }
}