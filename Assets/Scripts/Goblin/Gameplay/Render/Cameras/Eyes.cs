using Cinemachine;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Goblin.Gameplay.Render.Cameras
{
    /// <summary>
    /// 眼睛/镜头
    /// </summary>
    public class Eyes : Comp
    {
        /// <summary>
        /// 世界
        /// </summary>
        private World world { get; set; }
        /// <summary>
        /// 相机
        /// </summary>
        public Camera camera { get; private set; }
        /// <summary>
        /// CM 的 GameObject
        /// </summary>
        private GameObject cmgo { get; set; }
        /// <summary>
        /// CM 输入组件
        /// </summary>
        private CinemachineInputProvider cminput { get; set; }
        /// <summary>
        /// CM 组件
        /// </summary>
        private CinemachineFreeLook cm { get; set; }
        /// <summary>
        /// CM 偏移组件
        /// </summary>
        private CinemachineCameraOffset cmoff { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            world.ticker.eventor.Listen<TickEvent>(OnTick);
            camera = Camera.main;
            cmgo = new GameObject("CMFreeLock");
            
            cminput = cmgo.AddComponent<CinemachineInputProvider>();
            cminput.XYAxis = InputActionReference.Create(engine.u3dkit.gamepad.Player.Look);
            cminput.enabled = false;
            
            cm = cmgo.AddComponent<CinemachineFreeLook>();
            cmoff = cmgo.AddComponent<CinemachineCameraOffset>();
            
            // 设置 CM 相机参数
            cm.m_Lens.FieldOfView = 45f;
            cm.m_YAxis.m_InvertInput = true;
            cm.m_XAxis.m_InvertInput = false;
            cm.m_Orbits[0].m_Height = 4.5f;
            cm.m_Orbits[1].m_Height = 2.5f;
            cm.m_Orbits[2].m_Height = 0f;
            cm.m_Orbits[0].m_Radius = 4f;
            cm.m_Orbits[1].m_Radius = 4.5f;
            cm.m_Orbits[2].m_Radius = 4f;
            // 设置 CM 相机的 DeadZone (避免屏幕抖动)
            ModifyDeadZone(cm, 2, 2);
            
            // 设置 CM 相机偏移
            cmoff.m_Offset = new Vector3(0, 1.7f, -2);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.ticker.eventor.UnListen<TickEvent>(OnTick);
            GameObject.Destroy(cmgo);
        }

        public Eyes Initialize(World world)
        {
            this.world = world;

            return this;
        }

        private void OnTick(TickEvent e)
        {
            // 根据鼠标锁定状态来锁定 CM 输入组件
            cminput.enabled = CursorLockMode.Locked == Cursor.lockState;
            
            // LookAt 自己
            var node = world.GetAgent<NodeAgent>(world.self);
            if (null == node) return;
            cm.Follow = node.go.transform;
            cm.LookAt = node.go.transform;
            
            // 镜头推远推近
            var scroll = engine.u3dkit.gamepad.UI.ScrollWheel.ReadValue<Vector2>();
            if (0 != scroll.y)
            {
                var offset = cmoff.m_Offset;
                offset.z = Mathf.Clamp(offset.z + scroll.y, -7.5f, 0);
                cmoff.m_Offset = Vector3.Lerp(cmoff.m_Offset, offset, 0.1f);
            }
        }

        private void ModifyDeadZone(CinemachineFreeLook freeLookCamera, float deadZoneWidth, float deadZoneHeight)
        {
            // 遍历 CM FreeLook 的每个 Rig
            for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++)
            {
                var rig = freeLookCamera.GetRig(i);
                if (null == rig) continue;
                
                var composer = rig.GetCinemachineComponent<CinemachineComposer>();
                if (null == composer) continue;
                
                // 设置 DeadZone 宽度和高度
                composer.m_SoftZoneWidth = deadZoneWidth;
                composer.m_SoftZoneHeight = deadZoneHeight;
            }
        }
    }
}