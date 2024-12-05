using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Attributes.Surfaces;
using Goblin.Gameplay.Logic.Buffs.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Physics;
using Goblin.Gameplay.Logic.Physics.Common;
using Goblin.Gameplay.Logic.Skills;
using Goblin.Gameplay.Logic.States.Player;
using Goblin.Gameplay.Logic.Spatials;
using Kowtow;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Actors
{
    /// <summary>
    /// 玩家
    /// </summary>
    public class Player : Actor
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            AddBehavior<Attribute>().Create();
            AddBehavior<Surface>().Create();
            AddBehavior<BuffBucket>().Create();
            AddBehavior<Spatial>().Create();
            AddBehavior<PhysAgent>().Create();
            AddBehavior<Gamepad>().Create();
            AddBehavior<ParallelMachine>().Create();
            AddBehavior<SkillLauncher>().Create();

            // TODO 后续要改成配置文件读取
            var attribute = GetBehavior<Attribute>();
            attribute.hp = uint.MaxValue;
            attribute.maxhp = uint.MaxValue;
            attribute.movespeed = 20;
            attribute.jumpforce = 10;
            attribute.attack = 1000;

            var surface = GetBehavior<Surface>();
            surface.model = 10000;

            var spatial = GetBehavior<Spatial>();
            spatial.eulerAngles = new FPVector3(0, 90, 0);

            var physagent = GetBehavior<PhysAgent>();
            physagent.rigidbody.layer = Layer.Player;
            physagent.rigidbody.material = new Material { friction = 2, bounciness = FP.Zero };
            physagent.rigidbody.type = RigidbodyType.Dynamic;
            physagent.rigidbody.detection = DetectionType.Continuous;
            physagent.boxshape.size = new FPVector3(FP.Half, 15 * FP.EN1, FP.Half);
            physagent.rigidbody.gravityScale = 2;

            var paramachine = GetBehavior<ParallelMachine>();
            paramachine.SetState<PlayerFalling>();
            paramachine.SetState<PlayerFalling2Ground>();
            paramachine.SetState<PlayerJumpStart>();
            paramachine.SetState<PlayerJumping>();
            paramachine.SetState<PlayerRoll>();
            paramachine.SetState<PlayerHurt>();
            paramachine.SetState<PlayerAttack>();
            paramachine.SetState<PlayerIdle>();
            paramachine.SetState<PlayerRun>();
        }
    }
}
