using Goblin.Sys.Common;
using UnityEngine;

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
                var worldPos = engine.gameui.uicamera.ScreenToWorldPoint(e.position);
                worldPos.z = joystickBgPos.z;
                joystickBgRTF.position = worldPos;
            }, UIEventEnum.PointerDown);
            
            AddUIEventListener("JoystickArea", (e) =>
            {
                joystickBgRTF.position = joystickBgPos;
                joystickHandleTrans.position = joystickBgPos;
            }, UIEventEnum.PointerUp);
            
            AddUIEventListener("JoystickArea", (e) =>
            {
            }, UIEventEnum.BeginDrag);
            
            AddUIEventListener("JoystickArea", (e) =>
            {
                var pos = engine.gameui.uicamera.WorldToScreenPoint(joystickBgRTF.transform.position);
                var dir = (e.position - new Vector2(pos.x, pos.y)).normalized;
                var dis  = Mathf.Clamp(Vector2.Distance(e.position, new Vector2(pos.x, pos.y)), 0, joystickBgRadius);
                var handlePos = engine.gameui.uicamera.ScreenToWorldPoint(dir * dis + new Vector2(pos.x, pos.y));
                handlePos.z = pos.z;
                joystickHandleTrans.position = handlePos;
            }, UIEventEnum.Drag);
            
            AddUIEventListener("JoystickArea", (e) =>
            {

            }, UIEventEnum.EndDrag);
            
            AddUIEventListener("SplitBtn", (e) =>
            {
                Debug.Log("Split");
            }, UIEventEnum.PointerDown);
        }
    }
}
