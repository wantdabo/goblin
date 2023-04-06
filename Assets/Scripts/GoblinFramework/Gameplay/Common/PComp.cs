using GoblinFramework.Client.Gameplay;
using GoblinFramework.Core;

namespace GoblinFramework.Gameplay.Common
{
    public class PComp : Comp
    {
        public GameStage stage;

        protected override void OnCreate()
        {
            base.OnCreate();
            if (this is ILoop) stage.ticker.AddLoop(this as ILoop);
            if (this is ILateLoop) stage.ticker.AddLateLoop(this as ILateLoop);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this is ILoop) stage.ticker.RmvLoop(this as ILoop);
            if (this is ILateLoop) stage.ticker.RmvLateLoop(this as ILateLoop);
        }
    }
}