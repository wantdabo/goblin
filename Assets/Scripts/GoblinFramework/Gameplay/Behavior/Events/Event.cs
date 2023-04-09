using GoblinFramework.Core;

namespace GoblinFramework.Gameplay.Events
{
    public class Event
    {
        
    }

    public class TestEvent : Event
    {
        public string testStr;
    }

    #region SkillPipelineEvent
    public class SPBeginEvent : Event
    {
        public Actor caster;
    }
    
    public class SPCostEvent : Event
    {
        public Actor caster;
    }

    public class SPReadingEvent : Event
    {
        public Actor caster;
    }

    public class SPCastEvent : Event
    {
        public Actor caster;
    }

    public class SPProjectEvent : Event
    {
        public Actor caster;
    }

    public class SPHitEvent : Event
    {
        public Actor target;
    }
    
    public class SPEndEvent : Event
    {
        public Actor caster;
    }
    #endregion
}