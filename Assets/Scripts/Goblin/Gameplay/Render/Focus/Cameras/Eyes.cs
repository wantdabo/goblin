using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Focus.Common;
using UnityEngine;

namespace Goblin.Gameplay.Render.Focus.Cameras
{
    public class Eyes : Comp
    {
        public Foc foc { get; set; }

        private readonly Vector3 rightOffset = new(0, 3f, -10f);
        private readonly Vector3 leftOffset = new(0, 3f, -10f);
        private readonly float followSpeed = 4.5f;
        private (float, float) zoomRange = (5f, 45f);
        private readonly float zoomSpeed = 15f;
        
        private Camera camera { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            foc.stage.ticker.eventor.Listen<TickEvent>(OnTick);
            foc.stage.ticker.eventor.Listen<LateTickEvent>(OnLateTick);

            // 查询主相机
            foreach (var c in Camera.allCameras)
            {
                if (c.name.Contains("Eyes"))
                {
                    camera = c;
                    break;
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foc.stage.ticker.eventor.UnListen<TickEvent>(OnTick);
            foc.stage.ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
            camera = null;
        }
        
        private void OnTick(TickEvent e)
        {
            if (null == camera) return;
            if (Input.GetKey(KeyCode.Z))
            {
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView - e.tick * zoomSpeed, zoomRange.Item1, zoomRange.Item2);
            }
            else if (Input.GetKey(KeyCode.X))
            {
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView + e.tick * zoomSpeed, zoomRange.Item1, zoomRange.Item2);
            }
        }

        private void OnLateTick(LateTickEvent e)
        {
            if (null == camera) return;
            var actor = foc.stage.GetActor(foc.actorId);
            if (null == actor) return;
            var node = actor.GetBehavior<Node>();
            if (null == node || null == node.go) return;
            
            var offset = node.go.transform.rotation.eulerAngles.y > 90f ? leftOffset : rightOffset;
            camera.transform.position = Vector3.Lerp(camera.transform.position, node.go.transform.position + offset, followSpeed * Time.deltaTime);
        }
    }
}
