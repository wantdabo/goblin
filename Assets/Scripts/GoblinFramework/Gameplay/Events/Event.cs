using GoblinFramework.Core;
using GoblinFramework.Gameplay.Phys;
using GoblinFramework.Gameplay.Skills;

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

    public struct SkillPipelineStateEvent : Event
    {
        public Actor caster;
        public SkillPipelineState state;
    }
}