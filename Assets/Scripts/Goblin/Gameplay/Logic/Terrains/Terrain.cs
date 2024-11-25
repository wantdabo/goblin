using Goblin.Core;
using Goblin.Gameplay.Logic.Core;
using Kowtow;
using Kowtow.Collision.Shapes;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Terrains
{
    /// <summary>
    /// 地形
    /// </summary>
    public class Terrain : Comp
    {
        /// <summary>
        /// 场景
        /// </summary>
        public Stage stage { get; set; }
        /// <summary>
        /// 地板
        /// </summary>
        private Rigidbody ground { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            ground = stage.phys.AddBody(new BoxShape(FPVector3.zero, new FPVector3(1000, 1, 1000)), FP.One);
            ground.layer = Layer.Ground;
            ground.material = new Material { friction = 2, bounciness = FP.Zero };
            ground.position = FPVector3.down * 126 * FP.EN2;
        }
    }
}
