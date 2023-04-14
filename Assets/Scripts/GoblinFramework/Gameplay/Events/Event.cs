using System;
using GoblinFramework.Common.Events;
using GoblinFramework.Gameplay.Skills;

namespace GoblinFramework.Gameplay.Events
{
    public struct UpdateEvent : IEvent
    {
        public float tick;
    }

    public struct LateUpdateEvent : IEvent
    {
        public float tick;
    }

    public struct FixedUpdateEvent : IEvent
    {
        public float tick;
    }

    public struct TickEvent : IEvent 
    {
        public uint frame;
        public float tick;
    }

    public struct LateTickEvent : IEvent 
    {
        public uint frame;
        public float tick;
    }
    
    public struct GameStatusEvent : IEvent
    {
        public GameStatus state;
    }

    public struct AddActorEvent : IEvent
    {
        public uint actor;
    }

    public struct RmvActorEvent : IEvent
    {
        public uint actor;
    }

    public struct CollisionEnterEvent : IEvent
    {
        public uint actor;
    }

    public struct CollisionLeaveEvent : IEvent
    {
        public uint actor;
    }

    public struct StateChangeEvent : IEvent
    {
        public int layer;
        public Type type;
    }

    public struct SkillPipelineStateEvent : IEvent
    {
        public Actor caster;
        public SkillPipelineState state;
    }
}