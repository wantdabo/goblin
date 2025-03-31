using Goblin.Common;
using Goblin.Sys.Common;
using Goblin.Sys.Lobby.View;
using UnityEngine;
using UnityEngine.UI;

namespace Goblin.Sys.Gameplay.View
{
    public class GameplayView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;
        protected override string res => "Gameplay/GameplayView";

        protected override void OnBindEvent()
        {
            base.OnBindEvent();

            AddUIEventListener("ExitBtn", (e) =>
            {
                engine.proxy.gameplay.Stop();
                engine.gameui.Open<LobbyView>();
            });
        }
    }
}
