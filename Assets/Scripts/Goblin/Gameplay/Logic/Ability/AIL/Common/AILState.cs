namespace Goblin.Gameplay.Logic.Ability.AIL.Common
{
    public abstract class AILState
    {
        public void Reset()
        {
            OnReset();
        }
        
        public abstract void OnReset();
    }

    public sealed class AILState<T> : AILState where T : IAIL
    {
        public T inst { get; set; }

        public override void OnReset()
        {
            inst = default;
        }
    }
}