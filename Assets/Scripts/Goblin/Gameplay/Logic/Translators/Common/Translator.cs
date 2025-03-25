using Goblin.Gameplay.Logic.Core;
using Goblin.Sys.Initialize;

namespace Goblin.Gameplay.Logic.Translators.Common
{
    public abstract class Translator
    {
        public Stage stage { get; private set; }

        public void Initialize(Stage stage)
        {
            this.stage = stage;
        }

        public void RIL(BehaviorInfo info)
        {
            OnRIL(info);
        }

        protected virtual void OnRIL(BehaviorInfo info)
        {
        }
    }
    
    public abstract class Translator<T> : Translator where T : BehaviorInfo
    {
        protected override void OnRIL(BehaviorInfo info)
        {
            base.OnRIL(info);
            OnRIL((T)info);
        }
        
        protected abstract void OnRIL(T info);
    }
}