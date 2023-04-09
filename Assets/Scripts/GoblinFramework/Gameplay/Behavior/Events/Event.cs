using GoblinFramework.Core;
using GoblinFramework.Gameplay.Phys;

namespace GoblinFramework.Gameplay.Events
{
    public class Event
    {
        
    }

    public class TestEvent : Event
    {
        public string testStr;
    }

    public class CollisionEnterEvent : Event
    {
        public uint actorId;
    }

    public class CollisionLeaveEvent : Event
    {
        public uint actorId;
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