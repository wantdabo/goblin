using Goblin.Gameplay.Logic.Skills.ActionCache.Common;

namespace Goblin.Gameplay.Logic.Skills.ActionCache
{
    public class DetectionCache : SkillActionCache
    {
        public int detectedcnt { get; set; }

        public override void OnReset()
        {
            base.OnReset();
            detectedcnt = 0;
        }
    }
}
