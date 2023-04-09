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
    public class SPStartEvent : Event
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
    
    public class SPFinishEvent : Event
    {
        public Actor caster;
    }
    
    public class SPBreakEvent : Event
    {
        public Actor interrupter;
    }
    #endregion
}