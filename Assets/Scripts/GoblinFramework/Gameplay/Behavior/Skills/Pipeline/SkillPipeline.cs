using GoblinFramework.Core;
using GoblinFramework.Gameplay.Events;
using GoblinFramework.Gameplay.States;

namespace GoblinFramework.Gameplay.Skills.Pipeline
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

    public class SkillPipelineInfo : StateMachineInfo
    {
        public Actor caster;
    }

    public class SkillPipeline : Behavior<SkillPipelineInfo>
    {
        private SkillPipelineState mState;

        public SkillPipelineState state
        {
            get { return mState; }
            private set { mState = value; }
        }
        
        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.onTick += OnTick;
        }

        protected override void OnDestroy()
        {
            actor.stage.ticker.onTick -= OnTick;
            base.OnDestroy();
        }

        private void OnTick(int frame, float tick)
        {
            if (SkillPipelineState.Project != state) return;
            Project();
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

        private void Project()
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