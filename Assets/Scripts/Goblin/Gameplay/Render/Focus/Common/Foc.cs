using Goblin.Core;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Focus.Cameras;
using Goblin.Gameplay.Render.Focus.Lights;

namespace Goblin.Gameplay.Render.Focus.Common
{
    /// <summary>
    /// 专注/焦点
    /// </summary>
    public class Foc : Comp
    {
        public Stage stage { get; set; }
        public uint actorId { get; private set; }
        public Eyes eyes { get; private set; }
        public FocusLight light { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eyes = AddComp<Eyes>();
            eyes.foc = this;
            eyes.Create();
            
            light = AddComp<FocusLight>();
            light.foc = this;
            light.Create();
        }

        public void SetFollow(uint id)
        {
            actorId = id;
        }
    }
}
