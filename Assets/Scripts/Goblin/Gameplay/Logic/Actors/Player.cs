using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Attributes.Surfaces;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Physics;
using Goblin.Gameplay.Logic.Skills;
using Goblin.Gameplay.Logic.States.Player;
using Goblin.Gameplay.Logic.Spatials;
using TrueSync;

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
            AddBehavior<Spatial>().Create();
            AddBehavior<PhysAgent>().Create();
            AddBehavior<Gamepad>().Create();
            AddBehavior<ParallelMachine>().Create();
            AddBehavior<SkillLauncher>().Create();

            var attribute = GetBehavior<Attribute>();
            attribute.hp = uint.MaxValue;
            attribute.maxhp = uint.MaxValue;
            attribute.movespeed = 55 * FP.EN1;
            attribute.attack = 1000;
            
            var surface = GetBehavior<Surface>();
            surface.model = 10000;
            
            var spatial = GetBehavior<Spatial>();
            spatial.eulerAngle = new TSVector(0, 90, 0);
            
            var physagent = GetBehavior<PhysAgent>();
            physagent.rigidbodyoffset = new TSVector(0, 75 * FP.EN2, 0);
            physagent.boxshape.Size = new TSVector(1, 15 * FP.EN1, 1);
            physagent.trigger = true;
            
            var paramachine = GetBehavior<ParallelMachine>();
            paramachine.SetState<PlayerIdle>();
            paramachine.SetState<PlayerRun>();
            paramachine.SetState<PlayerHurt>();
            paramachine.SetState<PlayerAttack>();
            
            // TODO 后续要改成配置文件读取
            var launcher = GetBehavior<SkillLauncher>();
            launcher.Load(10001);
            launcher.Load(10002);
            launcher.Load(10003);
            launcher.Load(10004);
            launcher.Load(10011);
            launcher.Load(10012);
            launcher.Load(10013);
        }
    }
}
