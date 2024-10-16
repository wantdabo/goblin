using Goblin.Core;
using Goblin.Gameplay.Logic.Inputs;

namespace Goblin.Gameplay.Logic.Skills
{
    public class SkillNavLaunchType
    {
        public const int NONE = 0;
        public const int GAMEPAD = 1;
        public const int AI = 2;
    }

    public class SkillNavigation : Comp
    {
        public SkillLauncher launcher { get; set; }
        private Gamepad gamepad { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            gamepad = launcher.actor.GetBehavior<Gamepad>();
        }

        public void Launch(int type)
        {
            if (SkillNavLaunchType.NONE == type) return;
            
            if (SkillNavLaunchType.GAMEPAD == (SkillNavLaunchType.GAMEPAD & type))
            {
                // TODO 后续要改成配置文件读取
            }
            else if (SkillNavLaunchType.AI == (SkillNavLaunchType.AI & type))
            {
                // TODO 后续要改成配置文件读取
            }
        }
    }
}
