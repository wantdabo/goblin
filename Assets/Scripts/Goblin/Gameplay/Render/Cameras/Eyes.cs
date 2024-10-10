using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Cameras
{
    public class Eyes : Comp
    {
        private readonly Vector3 rightOffset = new(1f, 2.5f, -7.5f);
        private readonly Vector3 leftOffset = new(-1f, 2.5f, -7.5f);
        private readonly float speed = 3.5f;
        public Stage stage { get; set; }
        private Camera camera { get; set; }
        private uint actorID { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            stage.ticker.eventor.Listen<LateTickEvent>(OnLateTick);

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
            stage.ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
            camera = null;
        }

        public void SetFollow(uint id)
        {
            actorID = id;
        }

        private void OnLateTick(LateTickEvent e)
        {
            if (null == camera) return;
            var actor = stage.GetActor(actorID);
            if (null == actor) return;
            var node = actor.GetBehavior<Node>();
            if (null == node || null == node.go) return;
            
            var offset = node.go.transform.rotation.eulerAngles.y > 0 ? leftOffset : rightOffset;
            camera.transform.position = Vector3.Lerp(camera.transform.position, node.go.transform.position + offset, speed * Time.deltaTime);
        }
    }
}
