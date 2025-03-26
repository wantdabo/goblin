namespace Goblin.Gameplay.Logic.Core
{
    public abstract class Translator
    {
        public Stage stage { get; private set; }

        public void Initialize(Stage stage)
        {
            this.stage = stage;
        }

        public void RIL(ulong id, BehaviorInfo info)
        {
            OnRIL(id, info);
        }

        protected virtual void OnRIL(ulong id, BehaviorInfo info)
        {
        }
    }
    
    public abstract class Translator<T> : Translator where T : BehaviorInfo
    {
        protected override void OnRIL(ulong id, BehaviorInfo info)
        {
            base.OnRIL(id, info);
            OnRIL(id, (T)info);
        }
        
        protected abstract void OnRIL(ulong id, T info);
    }
}