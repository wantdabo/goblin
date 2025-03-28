namespace Goblin.Gameplay.Logic.Core
{
    public abstract class Translator
    {
        public Stage stage { get; private set; }

        public Translator Initialize(Stage stage)
        {
            this.stage = stage;

            return this;
        }

        public void Translate(ulong id, IBehaviorInfo info)
        {
            OnRIL(id, info);
        }

        protected virtual void OnRIL(ulong id, IBehaviorInfo info)
        {
        }
    }
    
    public abstract class Translator<T> : Translator where T : IBehaviorInfo
    {
        protected override void OnRIL(ulong id, IBehaviorInfo info)
        {
            base.OnRIL(id, info);
            OnRIL(id, (T)info);
        }
        
        protected abstract void OnRIL(ulong id, T info);
    }
}