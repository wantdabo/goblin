using Goblin.Common;
using Goblin.Gameplay.Render.Resolvers;
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

        protected override void OnLoad()
        {
            base.OnLoad();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
            engine.proxy.gameplay.eventor.Listen<SynopsisEvent>(OnSynopsis);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
            engine.proxy.gameplay.eventor.UnListen<SynopsisEvent>(OnSynopsis);
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            
            AddUIEventListener("EnterLockCursorBtn", (e) =>
            {
                Cursor.lockState = CursorLockMode.Locked;
            });
            
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

        private void OnTick(TickEvent e)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private void OnSynopsis(SynopsisEvent e)
        {
            var text = engine.u3dkit.SeekNode<Text>(gameObject, "Synopsis");
            var content = 
                          $"Frame : {e.synopsis.frame}\n" +
                          $"ActorCount : {e.synopsis.actorcnt}\n" +
                          $"BehaviorCount : {e.synopsis.behaviorcnt}\n" +
                          $"BehaviorInfoCount : {e.synopsis.behaviorinfocnt}\n" +
                          $"HasSnapshot : {e.synopsis.hassnapshot}\n" +
                          $"SnapshotFrame : {e.synopsis.snapshotframe}";
            text.text = content;
        }
    }
}
