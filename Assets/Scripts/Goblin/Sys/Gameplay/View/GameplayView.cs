using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Logic.Actors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Lives;
using Goblin.Sys.Common;
using TrueSync;
using UnityEngine;
using RStage = Goblin.Gameplay.Render.Core.Stage;

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
        private Vector3 joystickDir = Vector3.zero;
        private Stage stage { get; set; }
        private RStage rstage { get; set; }

        protected override void OnLoad()
        {
            base.OnLoad();
            Time.fixedDeltaTime = 1f / GameDef.LOGIC_FRAME;
            stage = AddComp<Stage>();
            stage.Create();
            rstage = AddComp<RStage>();
            rstage.Create();
            rstage.foc.SetFollow(1);

            stage.eventor.Listen<RILSyncEvent>(OnRILSync);
            var player = stage.AddActor<Player>();
            player.Create();
            player.eventor.Tell<LiveBornEvent>();

            engine.ticker.eventor.Listen<TickEvent>(OnTick);
            engine.ticker.eventor.Listen<FixedTickEvent>(OnFixedTick);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
            engine.ticker.eventor.UnListen<FixedTickEvent>(OnFixedTick);
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
            AddUIEventListener("JoystickArea", (e) =>
            {
                joystickFlag = true;
                var worldPos = engine.gameui.uicamera.ScreenToWorldPoint(e.position);
                worldPos.z = joystickBgPos.z;
                joystickBgRTF.position = worldPos;
                joystickDir = Vector3.zero;
            }, UIEventEnum.PointerDown);

            AddUIEventListener("JoystickArea", (e) =>
            {
                joystickFlag = false;
                joystickBgRTF.position = joystickBgPos;
                joystickHandleTrans.position = joystickBgPos;
                joystickDir = Vector3.zero;
            }, UIEventEnum.PointerUp);

            AddUIEventListener("JoystickArea", (e) =>
            {
            }, UIEventEnum.BeginDrag);

            AddUIEventListener("JoystickArea", (e) =>
            {
                var pos = engine.gameui.uicamera.WorldToScreenPoint(joystickBgRTF.transform.position);
                var dir = (e.position - new Vector2(pos.x, pos.y)).normalized;
                joystickDir = dir;
                var dis = Mathf.Clamp(Vector2.Distance(e.position, new Vector2(pos.x, pos.y)), 0, joystickBgRadius);
                var handlePos = engine.gameui.uicamera.ScreenToWorldPoint(dir * dis + new Vector2(pos.x, pos.y));
                handlePos.z = pos.z;
                joystickHandleTrans.position = handlePos;
            }, UIEventEnum.Drag);

            AddUIEventListener("JoystickArea", (e) =>
            {

            }, UIEventEnum.EndDrag);

            AddUIEventListener("SpeedBtn", (e) =>
            {
                Debug.Log("Speed Btn Down");
            }, UIEventEnum.PointerDown);

            AddUIEventListener("SpeedBtn", (e) =>
            {
                Debug.Log("Speed Btn Up");
            }, UIEventEnum.PointerUp);
        }

        private void OnRILSync(RILSyncEvent e)
        {
            rstage.rilsync.OnRILSync(e.id, e.frame, e.ril);
        }

        private void OnTick(TickEvent e)
        {
            rstage.Tick(e.tick);
            
            if (joystickFlag) return;
            
            joystickDir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) joystickDir += Vector3.up;
            if (Input.GetKey(KeyCode.S)) joystickDir += Vector3.down;
            if (Input.GetKey(KeyCode.A)) joystickDir += Vector3.left;
            if (Input.GetKey(KeyCode.D)) joystickDir += Vector3.right;
            joystickDir.Normalize();
        }

        private void OnFixedTick(FixedTickEvent e)
        {
            var dir = joystickDir * engine.cfg.float2Int;
            var tsdir = new TSVector2() { x = Mathf.CeilToInt(dir.x) * FP.EN3, y = Mathf.CeilToInt(dir.y) * FP.EN3 };
            var player = stage.GetActor(1);
            var gamepad = player.GetBehavior<Gamepad>();
            var joystick = new InputInfo() { press = tsdir != TSVector2.zero, dire = tsdir };
            gamepad.SetInput(InputType.Joystick, joystick);
            stage.Tick();
        }
    }
}
