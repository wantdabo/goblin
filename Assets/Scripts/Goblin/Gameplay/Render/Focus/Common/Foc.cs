using Goblin.Core;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Focus.Cameras;
using Goblin.Gameplay.Render.Focus.Lights;

namespace Goblin.Gameplay.Render.Focus.Common
{
    public class Foc : Comp
    {
        public Stage stage { get; set; }
        public uint actorId { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            var eyes = AddComp<Eyes>();
            eyes.foc = this;
            eyes.Create();
            
            var focusLight = AddComp<FocusLight>();
            focusLight.foc = this;
            focusLight.Create();
        }

        public void SetFollow(uint id)
        {
            actorId = id;
        }
    }
}
