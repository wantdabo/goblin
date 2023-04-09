using System.Collections.Generic;
using GoblinFramework.Core;
using GoblinFramework.Gameplay.Events;
using GoblinFramework.Gameplay.States;

namespace GoblinFramework.Gameplay.Skills
{
    public enum SkillPipelineState
    {
        None,
        Begin,
        Cost,
        Reading,
        Cast,
        Project,
        End
    }

    public class SkillPipelineInfo : BehaviorInfo
    {
        public Actor caster;
        public List<uint> concacts = new();
    }

    public class SkillPipeline : Behavior<SkillPipelineInfo>
    {
        private SkillPipelineState mState = SkillPipelineState.None;

        public SkillPipelineState state
        {
            get { return mState; }
            private set { mState = value; }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.eventor.Listen<CollisionEnterEvent>(OnCollisionEnter);
            actor.eventor.Listen<CollisionLeaveEvent>(OnCollisionLeave);
            actor.stage.ticker.onTick += OnTick;
        }

        protected override void OnDestroy()
        {
            actor.eventor.UnListen<CollisionEnterEvent>(OnCollisionEnter);
            actor.eventor.UnListen<CollisionLeaveEvent>(OnCollisionLeave);
            actor.stage.ticker.onTick -= OnTick;
            base.OnDestroy();
        }

        private void OnCollisionEnter(CollisionEnterEvent evt)
        {
            info.concacts.Add(evt.actorId);

            var e = new SPHitEvent();
            e.target = actor.stage.GetActor(evt.actorId);
            actor.eventor.Tell(evt);
        }

        private void OnCollisionLeave(CollisionLeaveEvent evt)
        {
            info.concacts.Remove(evt.actorId); 
        }

        private void OnTick(int frame, float tick)
        {
            if (SkillPipelineState.Project != state) return;
            Project(frame, tick);
        }

        public void Step()
        {
            state++;
            switch (state)
            {
                case SkillPipelineState.Begin:
                    Begin();
                    break;
                case SkillPipelineState.Cost:
                    Cost();
                    break;
                case SkillPipelineState.Reading:
                    Reading();
                    break;
                case SkillPipelineState.Cast:
                    Cast();
                    break;
                case SkillPipelineState.End:
                    End();
                    break;
            }
        }

        private void Begin()
        {
            state = SkillPipelineState.Begin;
            var e = new SPBeginEvent();
            e.caster = info.caster;
            actor.eventor.Tell(e);
        }

        private void Cost()
        {
            state = SkillPipelineState.Cost;
            var e = new SPCostEvent();
            e.caster = info.caster;
            actor.eventor.Tell(e);
        }

        private void Reading()
        {
            state = SkillPipelineState.Reading;
            var e = new SPReadingEvent();
            e.caster = info.caster;
            actor.eventor.Tell(e);
        }

        private void Cast()
        {
            state = SkillPipelineState.Cast;
            var e = new SPCastEvent();
            e.caster = info.caster;
            actor.eventor.Tell(e);
        }

        private void Project(int frame, float tick)
        {
            state = SkillPipelineState.Project;
            var e = new SPProjectEvent();
            e.caster = info.caster;
            actor.eventor.Tell(e);
        }

        private void End()
        {
            state = SkillPipelineState.End;
            var e = new SPEndEvent();
            e.caster = info.caster;
            actor.eventor.Tell(e);
        }
    }
}