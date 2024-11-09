using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Focus.Common;
using UnityEngine;

namespace Goblin.Gameplay.Render.Focus.Cameras
{
    /// <summary>
    /// Eyes/相机
    /// </summary>
    public class Eyes : Comp
    {
        /// <summary>
        /// 专注/焦点
        /// </summary>
        public Foc foc { get; set; }
        public Camera camera { get; private set; }

        private readonly Vector3 offset = new(0, 1.8f, -10f);
        private readonly float followSpeed = 2.5f;
        private (float min, float max) zoomRange = (2f, 4f);
        private readonly float zoomSpeed = 5f;

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
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - e.tick * zoomSpeed, zoomRange.min, zoomRange.max);
            }
            else if (Input.GetKey(KeyCode.X))
            {
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + e.tick * zoomSpeed, zoomRange.min, zoomRange.max);
            }
        }

        private void OnLateTick(LateTickEvent e)
        {
            if (null == camera) return;
            var actor = foc.stage.GetActor(foc.actorId);
            if (null == actor) return;
            var node = actor.GetBehavior<Node>();
            if (null == node || null == node.go) return;
            var o = Vector3.right  * (node.go.transform.eulerAngles.y > 91 ? -1f : 1f) + offset;
            camera.transform.position = Vector3.Lerp(camera.transform.position, node.go.transform.position + o, followSpeed * Time.deltaTime);
        }
    }
}
