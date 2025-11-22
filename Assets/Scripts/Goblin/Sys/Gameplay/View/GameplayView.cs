using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Director;
using Goblin.Gameplay.Logic.Commands;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Common.Extensions;
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
        private Toggle physDrawerToggle { get; set; }
        private Toggle showInfoCbToggle { get; set; }
        private Toggle danceCBToggle { get; set; }
        private Toggle enemyAutopoilotToggle { get; set; }
        private Transform selfSeatPoint { get; set; }
        private Transform infoContentTrans { get; set; }
        private Transform infoOrgTrans { get; set; }
        private List<Transform> infoItems { get; set; } = new();
        
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
            physDrawerToggle = engine.u3dkit.SeekNode<Toggle>(gameObject, "PhysDrawerCB");
            showInfoCbToggle = engine.u3dkit.SeekNode<Toggle>(gameObject, "ShowInfoCB");
            danceCBToggle = engine.u3dkit.SeekNode<Toggle>(gameObject, "DanceCB");
            enemyAutopoilotToggle = engine.u3dkit.SeekNode<Toggle>(gameObject, "EnemyAutopoilotCB");
            selfSeatPoint = engine.u3dkit.SeekNode<Transform>(gameObject, "SelfSeatPoint");
            infoContentTrans = engine.u3dkit.SeekNode<Transform>(gameObject, "InfoContent");
            infoOrgTrans = engine.u3dkit.SeekNode<Transform>(gameObject, "InfoOrgGo");
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
                var command = ObjectPool.Ensure<TimeScaleCommand>();
                command.timescale = (int)(timescale * 1000);
                localdirector.world.input.EnqueueCommand(command);
                
                gameSpeedDescText.text = timescale.ToString();
            });
            
            AddUIEventListener("GamingCB", (e) =>
            {
                if (gamingCBToggle.isOn)
                    engine.proxy.gameplay.director.ResumeGame(); 
                else 
                    engine.proxy.gameplay.director.PauseGame();
            });
            
            AddUIEventListener("PhysDrawerCB", (e) =>
            {
                engine.proxy.gameplay.physdraw = physDrawerToggle.isOn;
            });
            
            AddUIEventListener("ShowInfoCB", (e) =>
            {
                engine.proxy.gameplay.showinfo = showInfoCbToggle.isOn;
            });
            
            AddUIEventListener("DanceCB", (e) =>
            {
                engine.proxy.gameplay.dancing = danceCBToggle.isOn;
            });
            
            AddUIEventListener("EnemyAutopoilotCB", (e) =>
            {
                engine.proxy.gameplay.enemyautopilot = enemyAutopoilotToggle.isOn;
            });
            
            AddUIEventListener("SwitchSeatBtn", (e) =>
            {
                var seat = engine.proxy.gameplay.director.world.selfseat == 1 ? 2ul : 1ul;
                engine.proxy.gameplay.director.world.SwitchSeat(seat);
                engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = $"切换成功, 座位 {seat}" });
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
                engine.proxy.gameplay.UnLoad();
            });
        }

        private void OnLateTick(LateTickEvent e)
        {
            var localdirector = (engine.proxy.gameplay.director as LocalDirector);
            if (null == localdirector) return;
            
            // 设置主角指针位置
            var spatial = localdirector.world.rilbucket.GetRIL<RIL_SPATIAL>(localdirector.world.self);
            if (null != spatial)
            {
                var position = spatial.position.ToVector3();
                position.y += 2f;
                position = engine.u3dkit.WorldToUILoaclPoint(selfSeatPoint.parent.GetComponent<RectTransform>(), position);
                selfSeatPoint.localPosition = Vector3.Lerp(selfSeatPoint.localPosition, position, 0.1f);
            }

            foreach (var infoItem in infoItems) if (infoItem.gameObject.activeInHierarchy) infoItem.gameObject.SetActive(false);
            if (engine.proxy.gameplay.showinfo)
            {
                var rilactors = localdirector.world.rilbucket.GetRIL<RIL_ACTOR>(localdirector.world.sa);
                var cam = localdirector.world.eyes.camera;
                int index = 0;
                if (null == rilactors) return;
                foreach (var actor in rilactors.actors)
                {
                    var rilspatial = localdirector.world.rilbucket.GetRIL<RIL_SPATIAL>(actor);
                    if (null == rilspatial) continue;

                    var worldPos = rilspatial.position.ToVector3();
                    var vp = cam.WorldToViewportPoint(worldPos);
                    if (!(vp.z > 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f)) continue;

                    var rilstatemachine = localdirector.world.rilbucket.GetRIL<RIL_STATE_MACHINE>(actor);
                    var rilticker = localdirector.world.rilbucket.GetRIL<RIL_TICKER>(actor);
                    var rilattribute = localdirector.world.rilbucket.GetRIL<RIL_ATTRIBUTE>(actor);

                    var color = localdirector.world.self == actor ? "#D2FF00" : "#B90000";
                    string info = $"<color={color}>ACTOR : {actor}\n";
                    if (null != rilstatemachine)
                    {
                        info += $"当前状态 : {rilstatemachine.current}\n";
                        info += $"之前状态 : {rilstatemachine.last}\n";
                    }
                    
                    if (null != rilticker)
                    {
                        info += $"TIMESCALE : {rilticker.timescale}\n";
                    }
                    
                    if (null != rilattribute)
                    {
                        info += $"当前生命值 : {rilattribute.hp}\n";
                        info += $"最大生命值 : {rilattribute.maxhp}\n";
                        info += $"移动速度 : {rilattribute.movespeed}\n";
                        info += $"攻击力 : {rilattribute.attack}\n";
                    }
                    info += "</color>";

                    if (index >= infoItems.Count)
                    {
                        infoItems.Add(Object.Instantiate(infoOrgTrans, infoContentTrans));
                    }

                    var infoItem = infoItems[index];
                    infoItem.gameObject.SetActive(true);

                    var infoText = infoItem.GetComponent<Text>();
                    infoText.text = info;
                    index++;

                    // 跟随&缩放
                    worldPos.y += 1f;
                    float dist = Vector3.Distance(cam.transform.position, worldPos);
                    infoItem.localScale = 10f / (dist + 1f) * Vector3.one;

                    var rect = infoContentTrans.GetComponent<RectTransform>();
                    var uiPos = engine.u3dkit.WorldToUILoaclPoint(rect, worldPos);
                    infoItem.localPosition = Vector3.Lerp(infoItem.localPosition, uiPos, 0.2f);
                }
            }


            var rilstage = localdirector.world.rilbucket.GetRIL<RIL_STAGE>(localdirector.world.sa);
            var content =
                $"帧号 : {rilstage.frame}\n" +
                $"逻辑耗时 (毫秒) : {localdirector.stepms}\n" +
                $"Actor : {rilstage.actorcnt}\n" +
                $"Behavior : {rilstage.behaviorcnt}\n" +
                $"BehaviorInfo : {rilstage.behaviorinfocnt}\n";
            content += "存在快照 : " + (rilstage.hassnapshot ? "是\n" : "否\n");
            if (rilstage.hassnapshot) content += $"快照帧号 : {rilstage.snapshotframe}";
            
            synopsisText.text = content;
            
            if (false == rilstage.hassnapshot) return;
            if (rilstage.frame - rilstage.snapshotframe > 1) return;
            gameSpeedSlider.value = localdirector.timescale;
            gameSpeedDescText.text = localdirector.timescale.ToString();
        }
        
        private void OnEscape(InputAction.CallbackContext context)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
