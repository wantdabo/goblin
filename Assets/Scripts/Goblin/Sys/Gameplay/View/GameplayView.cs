﻿using Goblin.Common;
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

        private RectTransform joystickBgRTF;
        private Transform joystickHandleTrans;
        private Vector3 joystickBgPos;
        private float joystickBgRadius;

        private bool joystickFlag = false;
        private bool baFlag = false;
        private bool bbFlag = false;
        private bool bcFlag = false;
        private bool bdFlag = false;
        
        protected override void OnLoad()
        {
            base.OnLoad();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            joystickBgRTF = engine.u3dkit.SeekNode<RectTransform>(gameObject, "JoystickBg");
            joystickHandleTrans = engine.u3dkit.SeekNode<Transform>(gameObject, "JoystickHandle");
            joystickBgPos = joystickBgRTF.transform.position;
            joystickBgRadius = joystickBgRTF.sizeDelta.x / 2;
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            
            AddUIEventListener("ExitBtn", (e) =>
            {
                engine.proxy.gameplay.End();
                engine.gameui.Open<LobbyView>();
            });
            
            AddUIEventListener("GameSpeedSlider", (e) =>
            {
                var slider = engine.u3dkit.SeekNode<Slider>(gameObject, "GameSpeedSlider");
                engine.proxy.gameplay.gamespeed = (sbyte)slider.value;
                engine.u3dkit.SeekNode<Text>(gameObject, "GameSpeedDesc").text = engine.proxy.gameplay.gamespeed.ToString();
            });
            
            AddUIEventListener("GameSpeedSlider", (e) =>
            {
                var slider = engine.u3dkit.SeekNode<Slider>(gameObject, "GameSpeedSlider");
                engine.proxy.gameplay.gamespeed = (sbyte)slider.value;
                engine.u3dkit.SeekNode<Text>(gameObject, "GameSpeedDesc").text = engine.proxy.gameplay.gamespeed.ToString();
            }, UIEventEnum.EndDrag);
            
            AddUIEventListener("GamingCB", (e) =>
            {
                var toggle = engine.u3dkit.SeekNode<Toggle>(gameObject, "GamingCB");
                if (toggle.isOn) engine.proxy.gameplay.Resume(); else engine.proxy.gameplay.Pause();
            });
            
            AddUIEventListener("DanceCB", (e) =>
            {
                var toggle = engine.u3dkit.SeekNode<Toggle>(gameObject, "DanceCB");
                engine.proxy.gameplay.dancing = toggle.isOn;
            });
            
            AddUIEventListener("PhysDrawerCB", (e) =>
            {
                var toggle = engine.u3dkit.SeekNode<Toggle>(gameObject, "PhysDrawerCB");
                engine.proxy.gameplay.stage.physdrawer.draw = toggle.isOn;
            });

            AddUIEventListener("JoystickArea", (e) =>
            {
                joystickFlag = true;
                var worldPos = engine.gameui.uicamera.ScreenToWorldPoint(e.position);
                worldPos.z = joystickBgPos.z;
                joystickBgRTF.position = worldPos;
                engine.proxy.gameplay.input.joystickdire = Vector2.zero;
            }, UIEventEnum.PointerDown);

            AddUIEventListener("JoystickArea", (e) =>
            {
                joystickFlag = false;
                joystickBgRTF.position = joystickBgPos;
                joystickHandleTrans.position = joystickBgPos;
                engine.proxy.gameplay.input.joystickdire = Vector2.zero;
            }, UIEventEnum.PointerUp);

            AddUIEventListener("JoystickArea", (e) =>
            {
                var pos = engine.gameui.uicamera.WorldToScreenPoint(joystickBgRTF.transform.position);
                var dir = (e.position - new Vector2(pos.x, pos.y)).normalized;
                engine.proxy.gameplay.input.joystickdire = dir;
                var dis = Mathf.Clamp(Vector2.Distance(e.position, new Vector2(pos.x, pos.y)), 0, joystickBgRadius);
                var handlePos = engine.gameui.uicamera.ScreenToWorldPoint(dir * dis + new Vector2(pos.x, pos.y));
                handlePos.z = pos.z;
                joystickHandleTrans.position = handlePos;
            }, UIEventEnum.Drag);

            AddUIEventListener("AttackBtn", (e) =>
            {
                baFlag = true;
                engine.proxy.gameplay.input.bapress = true;
            }, UIEventEnum.PointerDown);

            AddUIEventListener("AttackBtn", (e) =>
            {
                baFlag = false;
                engine.proxy.gameplay.input.bapress = false;
            }, UIEventEnum.PointerUp);

            AddUIEventListener("SkillABtn", (e) =>
            {
                bbFlag = true;
                engine.proxy.gameplay.input.bbpress = true;
            }, UIEventEnum.PointerDown);

            AddUIEventListener("SkillABtn", (e) =>
            {
                bbFlag = false;
                engine.proxy.gameplay.input.bbpress = false;
            }, UIEventEnum.PointerUp);

            AddUIEventListener("SkillBBtn", (e) =>
            {
                bcFlag = true;
                engine.proxy.gameplay.input.bcpress = true;
            }, UIEventEnum.PointerDown);

            AddUIEventListener("SkillBBtn", (e) =>
            {
                bcFlag = false;
                engine.proxy.gameplay.input.bcpress = false;
            }, UIEventEnum.PointerUp);
            
            AddUIEventListener("SkillCBtn", (e) =>
            {
                bdFlag = true;
                engine.proxy.gameplay.input.bdpress = true;
            }, UIEventEnum.PointerDown);

            AddUIEventListener("SkillCBtn", (e) =>
            {
                bdFlag = false;
                engine.proxy.gameplay.input.bdpress = false;
            }, UIEventEnum.PointerUp);
        }

        private void OnTick(TickEvent e)
        {
            if (false == joystickFlag)
            {
                var joystickDir = Vector3.zero;
                if (Input.GetKey(KeyCode.W)) joystickDir += Vector3.up;
                if (Input.GetKey(KeyCode.S)) joystickDir += Vector3.down;
                if (Input.GetKey(KeyCode.A)) joystickDir += Vector3.left;
                if (Input.GetKey(KeyCode.D)) joystickDir += Vector3.right;
                joystickDir.Normalize();
                engine.proxy.gameplay.input.joystickdire = joystickDir;
            }
            
            // 跳跃
            if (false == baFlag)
            {
                engine.proxy.gameplay.input.bapress = Input.GetKey(KeyCode.Space);
            }
            // 翻滚
            if (false == bbFlag)
            {
                engine.proxy.gameplay.input.bbpress = Input.GetKey(KeyCode.L);
            }
            // 攻击
            if (false == bcFlag)
            {
                engine.proxy.gameplay.input.bcpress = Input.GetKey(KeyCode.J);
            }
            if (false == bdFlag)
            {
                engine.proxy.gameplay.input.bdpress = Input.GetKey(KeyCode.K);
            }
        }
    }
}
