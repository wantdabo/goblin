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
        /// <summary>
        /// 箱子 1
        /// </summary>
        private Rigidbody box1 { get; set; }
        /// <summary>
        /// 箱子 2
        /// </summary>
        private Rigidbody box2 { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            ground = stage.phys.AddBody(new BoxShape(FPVector3.zero, new FPVector3(1000, 1, 1000)), FP.One);
            ground.layer = Layer.Ground;
            ground.material = new Material { friction = 2, bounciness = FP.Zero };
            ground.position = FPVector3.down * FP.Half;
            
            box1 = stage.phys.AddBody(new BoxShape(FPVector3.zero, new FPVector3(1, 1, 1)), FP.One);
            box1.layer = Layer.Ground;
            box1.material = new Material { friction = 2, bounciness = FP.Zero };
            box1.position = new FPVector3(-3, FP.Half, 0);
            
            box2 = stage.phys.AddBody(new BoxShape(FPVector3.zero, new FPVector3(3, FP.Half, 3)), FP.One);
            box2.layer = Layer.Ground;
            box2.material = new Material { friction = 2, bounciness = FP.Zero };
            box2.position = new FPVector3(3, 2, FP.Zero);
        }
    }
}
