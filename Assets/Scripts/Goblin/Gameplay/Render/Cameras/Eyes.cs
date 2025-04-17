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
        private World world { get; set; }
        public Camera camera { get; private set; }
        private GameObject cmgo { get; set; }
        private CinemachineInputProvider cminput { get; set; }
        private CinemachineFreeLook cm { get; set; }
        private CinemachineCameraOffset cmoff { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            world.ticker.eventor.Listen<TickEvent>(OnTick);
            camera = Camera.main;
            cmgo = new GameObject("CMFreeLock");
            cminput = cmgo.AddComponent<CinemachineInputProvider>();
            cminput.XYAxis = InputActionReference.Create(engine.u3dkit.gamepad.Player.Look);
            cm = cmgo.AddComponent<CinemachineFreeLook>();
            cmoff = cmgo.AddComponent<CinemachineCameraOffset>();
            
            cm.m_Lens.FieldOfView = 45f;
            cm.m_YAxis.m_InvertInput = true;
            cm.m_XAxis.m_InvertInput = false;
            cm.m_Orbits[0].m_Height = 4.5f;
            cm.m_Orbits[1].m_Height = 2.5f;
            cm.m_Orbits[2].m_Height = 0f;
            cm.m_Orbits[0].m_Radius = 4f;
            cm.m_Orbits[1].m_Radius = 4.5f;
            cm.m_Orbits[2].m_Radius = 4f;
            ModifyDeadZone(cm, 2, 2);
            
            cmoff.m_Offset = new Vector3(0, 1.7f, 0);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.ticker.eventor.UnListen<TickEvent>(OnTick);
            camera = null;
            GameObject.Destroy(cmgo);
            cmgo = null;
            cm = null;
        }

        public Eyes Initialize(World world)
        {
            this.world = world;

            return this;
        }

        private void OnTick(TickEvent e)
        {
            var node = world.GetAgent<Node>(world.self);
            if (null == node) return;
            cm.Follow = node.go.transform;
            cm.LookAt = node.go.transform;
            
            var scroll = engine.u3dkit.gamepad.UI.ScrollWheel.ReadValue<Vector2>();
            if (0 != scroll.y)
            {
                var offset = cmoff.m_Offset;
                offset.z = Mathf.Clamp(offset.z + scroll.y * e.tick, -7.5f, 0);
                cmoff.m_Offset = offset;
            }
        }
        
        private void ModifyDeadZone(CinemachineFreeLook freeLookCamera, float deadZoneWidth, float deadZoneHeight)
        {
            // 遍历 CinemachineFreeLook 的每个 Rig
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