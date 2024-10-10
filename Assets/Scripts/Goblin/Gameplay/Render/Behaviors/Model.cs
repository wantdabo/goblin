using Goblin.Common;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Behaviors
{
    public struct ModelChangedEvent : IEvent
    {
    }

    public class Model : Behavior
    {
        public string res { get; private set; }
        public GameObject model { get; private set; }
        private Node node { get; set; }

        private static GameObject modelpool = new("MODELPOOL");
        static Model()
        {
            modelpool.SetActive(false);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            node = actor.EnsureBehavior<Node>();
        }

        public void Load(string res)
        {
            if (null != this.res && res.Equals(this.res)) return;
            if (null != model) engine.pool.Set($"MODEL_GO_KEY_{res}", model, (go) => go.transform.SetParent(modelpool.transform));
            this.res = res;

            model = engine.pool.Get<GameObject>($"MODEL_GO_KEY_{res}");
            if (null == model) model = engine.gameres.location.LoadModelSync(res);
            if (null == model) return;
            if (null == node || null == node.go)
            {
                engine.pool.Set($"MODEL_GO_KEY_{res}", model, (go) => go.transform.SetParent(modelpool.transform));
                return;
            }

            model.transform.SetParent(node.go.transform, false);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            actor.eventor.Tell<ModelChangedEvent>();
        }
    }
}
