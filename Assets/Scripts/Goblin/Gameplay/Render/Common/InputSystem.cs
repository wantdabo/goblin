using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Commands.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Render.Core;
using Kowtow.Math;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
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
        private Dictionary<ushort, (bool press, IntVector2 dire)> inputdict { get; set; }
        /// <summary>
        /// 输入指令队列
        /// </summary>
        private Queue<Command> cmdqueue { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            world.ticker.eventor.Listen<TickEvent>(OnTick);
            inputdict = ObjectPool.Ensure<Dictionary<ushort, (bool press, IntVector2 dire)>>();
            cmdqueue = ObjectPool.Ensure<Queue<Command>>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.ticker.eventor.UnListen<TickEvent>(OnTick);
            inputdict.Clear();
            ObjectPool.Set(inputdict);
            
            foreach (var command in cmdqueue)
            {
                command.Reset();
                ObjectPool.Set(command);
            }
            cmdqueue.Clear();
            ObjectPool.Set(cmdqueue);
        }

        /// <summary>
        /// 初始化输入系统
        /// </summary>
        /// <param name="world">世界</param>
        /// <returns>输入系统</returns>
        public InputSystem Initialize(World world)
        {
            this.world = world;

            return this;
        }
        
        /// <summary>
        /// 获取输入
        /// </summary>
        /// <param name="type">输入类型</param>
        /// <returns>输入信息</returns>
        public (bool press, IntVector2 dire) GetInput(ushort type)
        {
            if (inputdict.TryGetValue(type, out var input))
            {
                return input;
            }
            
            return default;
        }

        /// <summary>
        /// 设置输入
        /// </summary>
        /// <param name="type">输入类型</param>
        /// <param name="press">长按</param>
        /// <param name="dire">方向</param>
        public void SetInput(ushort type, bool press, IntVector2 dire)
        {
            if (inputdict.ContainsKey(type)) inputdict.Remove(type);
            
            inputdict.Add(type, (press, dire));
        }
        
        /// <summary>
        /// 尝试出队指令
        /// </summary>
        /// <param name="command">输入指令</param>
        /// <returns>YES/NO</returns>
        public bool TryDequeueCommand(out Command command)
        {
            return cmdqueue.TryDequeue(out command);
        }

        /// <summary>
        /// 设置输入指令
        /// </summary>
        /// <param name="command">输入指令</param>
        public void EnqueueCommand(Command command)
        {
            if (null == command) return;

            cmdqueue.Enqueue(command);
        }

        private void OnTick(TickEvent e)
        {
            if (CursorLockMode.Locked != Cursor.lockState) return;
            
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
            
            SetInput(INPUT_DEFINE.JOYSTICK, Vector2.zero != joystickdire, new IntVector2((int)(joystickdire.x * Config.Float2Int), (int)(joystickdire.y * Config.Float2Int)));
            
            var leftclick = engine.u3dkit.gamepad.Player.Fire.ReadValue<float>();
            SetInput(INPUT_DEFINE.BA, leftclick > 0, new IntVector2(0, 0));
        }
    }
}
