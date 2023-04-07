using GoblinFramework.Core;
using GoblinFramework.Gameplay.Common;

namespace GoblinFramework.Gameplay
{
    public class BehaviorInfo : Comp
    {
        public Behavior behavior;
    }
    
    public class Behavior : PComp
    {
        public Actor actor;
    }
    
    public class Behavior<T> : Behavior where T : BehaviorInfo, new()
    {
        public T info;

        protected override void OnCreate()
        {
            base.OnCreate();

            info = AddComp<T>();
            info.behavior = this;
            info.Create();
        }

        protected override void OnDestroy()
        {
            RmvComp(info);
            info.Destroy();
            
            base.OnDestroy();
        }
    }
}