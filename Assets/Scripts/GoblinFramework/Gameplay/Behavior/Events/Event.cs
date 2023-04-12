using GoblinFramework.Core;
using GoblinFramework.Gameplay.Phys;

namespace GoblinFramework.Gameplay.Events
{
    public interface Event
    {
        
    }

    public struct TestEvent : Event
    {
        public string testStr;
    }

    public struct TickEvent : Event 
    {
        public uint frame;
        public float tick;
    }

    public struct LateTickEvent : Event 
    {
        public uint frame;
        public float tick;
    }

    public struct CollisionEnterEvent : Event
    {
        public uint actorId;
    }

    public struct CollisionLeaveEvent : Event
    {
        public uint actorId;
    }

    #region SkillPipelineEvent
    public struct SPBeginEvent : Event
    {
        public Actor caster;
    }
    
    public struct SPCostEvent : Event
    {
        public Actor caster;
    }

    public struct SPReadingEvent : Event
    {
        public Actor caster;
    }

    public struct SPCastEvent : Event
    {
        public Actor caster;
    }

    public struct SPProjectEvent : Event
    {
        public Actor caster;
    }

    public struct SPHitEvent : Event
    {
        public Actor target;
    }
    
    public struct SPEndEvent : Event
    {
        public Actor caster;
    }
    #endregion
}