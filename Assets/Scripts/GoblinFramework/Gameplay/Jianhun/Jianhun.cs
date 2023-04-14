using GoblinFramework.Gameplay.Inputs;
using GoblinFramework.Gameplay.Skills;
using GoblinFramework.Gameplay.States;

namespace GoblinFramework.Gameplay
{
    public class Jianhun : Actor
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            var gamepad = AddBehavior<Gamepad>();
            gamepad.Create();
            
            var stateMachine = AddBehavior<StateMachine>();
            stateMachine.SetState<JianhunIdle>();
            stateMachine.SetState<JianhunRun>();
            stateMachine.Create();

            var skillLauncher = AddBehavior<SkillLauncher>();
            skillLauncher.Create();
        }
    }
}