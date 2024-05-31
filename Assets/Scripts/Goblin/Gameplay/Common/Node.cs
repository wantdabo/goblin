using Goblin.Common;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

namespace Goblin.Gameplay.Common
{
    /// <summary>
    /// 节点
    /// </summary>
    public class Node : Behavior
    {
        /// <summary>
        /// 坐标，PS 如果需要设计成服务端能运行，校验，需要更深层次剥离逻辑渲染，此处不分离，时间有限
        /// </summary>
        public TSVector2 pos { get; set; } = TSVector2.zero;

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
            engine.ticker.eventor.Listen<LateTickEvent>(OnLateTick);
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
            engine.ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
            engine.pool.Set("NODE_GO_KEY", gameObject, (go) =>
            {
                if (null == go) return;
                go.transform.localScale = Vector3.one;
                go.SetActive(false);
            });
            gameObject = null;
        }

        private void OnLateTick(LateTickEvent e)
        {
            Vector3 upos = new Vector3(pos.x.AsFloat(), pos.y.AsFloat(), 0);
            //gameObject.transform.position = upos;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, upos, 20 * e.tick);
        }
    }
}