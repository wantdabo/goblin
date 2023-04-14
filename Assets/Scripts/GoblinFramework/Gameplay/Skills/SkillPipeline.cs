using GoblinFramework.Common.Events;
using GoblinFramework.Core;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Events;

namespace GoblinFramework.Gameplay.Skills
{
    public enum SkillPipelineState
    {
        None,
        Start,
        Chant,
        Cast,
        Casting,
        Finish,
    }

    public abstract class SkillPipeline : Comp
    {
        public SkillLauncher launcher;

        private SkillPipelineState mState;
        public SkillPipelineState state
        {
            get { return mState; }
            private set { mState = value; }
        }

        public void Launch()
        {
            state = SkillPipelineState.None;
            NextStep();
        }

        protected void NextStep()
        {
            state++;
            switch (state)
            {
                case SkillPipelineState.Start:
                    OnStart();
                    break;
                case SkillPipelineState.Chant:
                    OnChant();
                    break;
                case SkillPipelineState.Cast:
                    OnCast();
                    break;
                case SkillPipelineState.Casting:
                    OnCasting();
                    break;
                case SkillPipelineState.Finish:
                    OnFinish();
                    break;
            }

            launcher.actor.eventor.Tell(new SkillPipelineStateEvent() { caster = launcher.actor, state = state });
        }

        protected abstract void OnStart();
        protected abstract void OnChant();
        protected abstract void OnCast();
        protected abstract void OnCasting();
        protected abstract void OnFinish();
    }
}