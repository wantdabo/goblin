using Goblin.Common;
using Goblin.Gameplay.Director;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Sys.Common;
using Goblin.Sys.Lobby.View;
using Goblin.Sys.Other.View;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Goblin.Sys.Gameplay.View
{
    public class GameplayView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;
        
        protected override string res => "Gameplay/GameplayView";
        
        private Text synopsisText { get; set; }
        private Slider gameSpeedSlider { get; set; }
        private Text gameSpeedDescText { get; set; }
        private Toggle gamingCBToggle { get; set; }
        
        protected override void OnLoad()
        {
            base.OnLoad();
            engine.ticker.eventor.Listen<LateTickEvent>(OnLateTick);
            engine.u3dkit.gamepad.UI.Escape.performed += OnEscape;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
            engine.u3dkit.gamepad.UI.Escape.performed -= OnEscape;
        }

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            synopsisText = engine.u3dkit.SeekNode<Text>(gameObject, "Synopsis");
            gameSpeedSlider = engine.u3dkit.SeekNode<Slider>(gameObject, "GameSpeedSlider");
            gameSpeedDescText = engine.u3dkit.SeekNode<Text>(gameObject, "GameSpeedDesc");
            gamingCBToggle = engine.u3dkit.SeekNode<Toggle>(gameObject, "GamingCB");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            AddUIEventListener("EnterLockCursorBtn", (e) =>
            {
                Cursor.lockState = CursorLockMode.Locked;
            });
            
            gameSpeedSlider.onValueChanged.AddListener((e) =>
            {
                var localdirector = (engine.proxy.gameplay.director as LocalDirector);
                if (null == localdirector) return;
                var timescale = Mathf.Round(gameSpeedSlider.value / 0.25f) * 0.25f;
                localdirector.timescale = timescale;
                gameSpeedDescText.text = timescale.ToString();
            });
            
            AddUIEventListener("GamingCB", (e) =>
            {
                if (gamingCBToggle.isOn)
                    engine.proxy.gameplay.director.ResumeGame(); 
                else 
                    engine.proxy.gameplay.director.PauseGame();
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
                engine.gameui.Close(this);
                engine.gameui.Open<LobbyView>();
                engine.proxy.gameplay.director.StopGame();
                engine.proxy.gameplay.director.DestroyGame();
            });
        }

        private void OnLateTick(LateTickEvent e)
        {
            if (null == synopsisText) return;
            if (null == engine.proxy.gameplay.director) return;
            var ril = engine.proxy.gameplay.director.world.rilbucket.GetRIL<RIL_STAGE>(engine.proxy.gameplay.director.world.sa);
            if (null == ril) return;
            
            var localdirector = (engine.proxy.gameplay.director as LocalDirector);
            if (null == localdirector) return;
            
            var content =
                $"帧号 : {ril.frame}\n" +
                $"逻辑耗时 (毫秒) : {localdirector.stepms}\n" +
                $"Actor : {ril.actorcnt}\n" +
                $"Behavior : {ril.behaviorcnt}\n" +
                $"BehaviorInfo : {ril.behaviorinfocnt}\n";
            content += "存在快照 : " + (ril.hassnapshot ? "是\n" : "否\n");
            if (ril.hassnapshot) content += $"快照帧号 : {ril.snapshotframe}";
            
            synopsisText.text = content;
            
            if (false == ril.hassnapshot) return;
            if (ril.frame - ril.snapshotframe > 1) return;
            gameSpeedSlider.value = localdirector.timescale;
            gameSpeedDescText.text = localdirector.timescale.ToString();
        }
        
        private void OnEscape(InputAction.CallbackContext context)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
