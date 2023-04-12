using GoblinFramework.Core;
using GoblinFramework.Gameplay.Common;

namespace GoblinFramework.Gameplay.Skills
{
    public class SkillPipeline : Comp
    {
        public SkillLauncher launcher;

        public void Launch() 
        {
            var timer = AddComp<Timer>();
            timer.Create(launcher.actor.stage.ticker);
            timer.Create();
        }
    }
}