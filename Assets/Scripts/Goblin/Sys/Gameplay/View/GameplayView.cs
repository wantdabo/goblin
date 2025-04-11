using Goblin.Common;
using Goblin.Sys.Common;
using Goblin.Sys.Lobby.View;
using Goblin.Sys.Other.View;
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
            
            AddUIEventListener("SnapshotBtn", (e) =>
            {
                engine.proxy.gameplay.director.Snapshot();
                engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "快照拍摄成功." });
            });
            
            AddUIEventListener("RestoreBtn", (e) =>
            {
                engine.proxy.gameplay.director.Restore();
                engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "快照恢复成功." });
            });

            AddUIEventListener("ExitBtn", (e) =>
            {
                engine.gameui.Open<LobbyView>();
                engine.proxy.gameplay.director.StopGame();
                engine.proxy.gameplay.director.DestroyGame();
            });
        }
    }
}
