using Goblin.Common;
using Goblin.Gameplay.Common;
using Goblin.Gameplay.Core;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common
{
    /// <summary>
    /// 节点
    /// </summary>
    public class Node : Behavior
    {
        /// <summary>
        /// U3D 节点
        /// </summary>
        public GameObject gameObject { get; private set; }

        /// <summary>
        /// Node 的根节点
        /// </summary>
        private static GameObject nodesGo = new("NODES");

        protected override void OnCreate()
        {
            base.OnCreate();
            gameObject = engine.pool.Get<GameObject>("NODE_GO_KEY", (go) => go.SetActive(true));
            if (null == gameObject) gameObject = new();
            gameObject.name = actor.id.ToString();
            gameObject.transform.SetParent(nodesGo.transform);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.pool.Set("NODE_GO_KEY", gameObject, (go) =>
            {
                if (null == go) return;
                go.transform.localScale = Vector3.one;
                go.SetActive(false);
            });
            gameObject = null;
        }
    }
}
