using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Focus.Common;
using UnityEngine;

namespace Goblin.Gameplay.Render.Focus.Lights
{
    public class FocusLight : Comp
    {
        private GameObject go { get; set; }
        public Foc foc { get; set; }
        
        protected override void OnCreate()
        {
            base.OnCreate();
            foc.stage.ticker.eventor.Listen<LateTickEvent>(OnLateTick);
            
            go = new GameObject("FocusLight");
            go.transform.rotation = Quaternion.Euler(90, 0, 0);
            var light = go.AddComponent<Light>();
            light.type = LightType.Spot;
            light.range = 15;
            light.spotAngle = 50f;
            light.intensity = 2f;
            light.shadows = LightShadows.Hard;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foc.stage.ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
        }
        
        private void OnLateTick(LateTickEvent e)
        {
            if (null == go) return;
            var actor = foc.stage.GetActor(foc.actorId);
            if (null == actor) return;
            var node = actor.GetBehavior<Node>();
            if (null == node || null == node.go) return;

            go.transform.position = node.go.transform.position + Vector3.up * 5f;
        }
    }
}
