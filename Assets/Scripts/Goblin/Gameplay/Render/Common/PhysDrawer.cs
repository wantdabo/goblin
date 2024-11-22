using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Physics.Common;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Kowtow.Collision.Shapes;
using ShapeDrawers.Common;
using UnityEngine;

namespace Goblin.Gameplay.Render.Common
{
    /// <summary>
    /// 物理绘制器
    /// </summary>
    public class PhysDrawer : Comp
    {
        /// <summary>
        /// 场景
        /// </summary>
        public Stage stage { get; set; }

        /// <summary>
        /// 开启绘制
        /// </summary>
        public bool draw { get; set; } = false;

        protected override void OnCreate()
        {
            base.OnCreate();
            stage.eventor.Listen<PhysShapesEvent>(OnPhysShapes);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stage.eventor.UnListen<PhysShapesEvent>(OnPhysShapes);
        }

        private void OnPhysShapes(PhysShapesEvent e)
        {
            ShapeDrawer.Clear();

            if (false == draw) return;

            foreach (var physinfo in e.physinfos)
            {
                Color color = default;
                switch (physinfo.type)
                {
                    case PHYS_SHAPE_DEFINE.PLAYER:
                        color = Color.green;
                        break;
                    case PHYS_SHAPE_DEFINE.OVERLAP:
                        color = Color.yellow;
                        break;
                }
                
                if (physinfo.shape is BoxShape boxshape)
                {
                    ShapeDrawer.DrawBox(physinfo.position.ToVector3(), boxshape.size.ToVector3(), physinfo.rotation.ToQuaternion(), color);
                }
                else if (physinfo.shape is SphereShape sphereshape)
                {
                    ShapeDrawer.DrawSphere(physinfo.position.ToVector3(), sphereshape.radius.AsFloat(), color);

                }
                else if (physinfo.shape is CylinderShape cylindershape)
                {
                    ShapeDrawer.DrawCylinder(physinfo.position.ToVector3(), cylindershape.radius.AsFloat(), cylindershape.height.AsFloat(), physinfo.rotation.ToQuaternion(), color);
                }
            }
        }
    }
}
