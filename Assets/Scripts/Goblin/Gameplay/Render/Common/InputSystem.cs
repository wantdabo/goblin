using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Render.Core;
using Kowtow.Math;
using UnityEngine;
using Config = Goblin.Common.Config;

namespace Goblin.Gameplay.Render.Common
{
    /// <summary>
    /// 输入系统
    /// </summary>
    public class InputSystem : Comp
    {
        /// <summary>
        /// 渲染世界
        /// </summary>
        private World world { get; set; }
        /// <summary>
        /// 输入数据集合
        /// </summary>
        public Dictionary<InputType, (bool press, GPVector2 dire)> inputdict { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            world.ticker.eventor.Listen<TickEvent>(OnTick);
            inputdict = ObjectCache.Get<Dictionary<InputType, (bool press, GPVector2 dire)>>();
            inputdict.Add(InputType.Joystick, default);
            inputdict.Add(InputType.BA, default);
            inputdict.Add(InputType.BB, default);
            inputdict.Add(InputType.BC, default);
            inputdict.Add(InputType.BD, default);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.ticker.eventor.UnListen<TickEvent>(OnTick);
            inputdict.Clear();
            ObjectCache.Set(inputdict);
        }

        public InputSystem Initialize(World world)
        {
            this.world = world;

            return this;
        }
        
        public (bool press, GPVector2 dire) GetInput(InputType type)
        {
            if (inputdict.TryGetValue(type, out var input))
            {
                return input;
            }
            
            return default;
        }

        public void SetInput(bool press, GPVector2 dire)
        {
            inputdict.Remove(InputType.Joystick);
            inputdict.Add(InputType.Joystick, (press, dire));
        }

        private void OnTick(TickEvent e)
        {
            var joystickdire = engine.u3dkit.gamepad.Player.Move.ReadValue<Vector2>();
            // 根据摄像机方向计算世界坐标系中的方向
            if (Vector2.zero != joystickdire && null != world.eyes.camera)
            {
                var cameraTransform = world.eyes.camera.transform;
                // 获取摄像机的前向和右向向量
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;

                // 忽略 Y 轴分量，保持平面运动
                forward.y = 0;
                right.y = 0;

                forward.Normalize();
                right.Normalize();

                // 将 joystickdire 转换为世界方向
                Vector3 worldDirection = joystickdire.x * right + joystickdire.y * forward;
                joystickdire = new Vector2(worldDirection.x, worldDirection.z);
            }

            SetInput
            (
                Vector2.zero != joystickdire,
                new GPVector2((int)(joystickdire.x * Config.Float2Int), (int)(joystickdire.y * Config.Float2Int))
            );
        }
    }
}
