using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;
using UnityEngine;

namespace Goblin.Sys.Gameplay
{
    /// <summary>
    /// 输入系统
    /// </summary>
    public class InputSystem : Comp
    {
        /// <summary>
        /// 摇杆方向
        /// </summary>
        public Vector2 joystickdire { get; set; }
        /// <summary>
        /// 按键 A PRESS
        /// </summary>
        public bool bapress { get; set; }
        /// <summary>
        /// 按键 B PRESS
        /// </summary>
        public bool bbpress { get; set; }
        /// <summary>
        /// 按键 C PRESS
        /// </summary>
        public bool bcpress { get; set; }
        /// <summary>
        /// 按键 D PRESS
        /// </summary>
        public bool bdpress { get; set; }
        
        /// <summary>
        /// 输入
        /// </summary>
        /// <param name="actorId">ActorID</param>
        /// <param name="stage">逻辑场景</param>
        public void Input(uint actorId, Stage stage)
        {
            var player = stage.GetActor(actorId);
            var gamepad = player.GetBehavior<Gamepad>();
            
            var dir = joystickdire;
            if (Vector2.zero != dir) if (dir.x > 0) dir.x = 1 * Config.float2Int; else dir.x = -1 * Config.float2Int;
            dir.y = 0;
            
            var tsdir = new FPVector2 { x = Mathf.CeilToInt(dir.x) * FP.EN3, y = FP.Zero };
            var joystick = new InputInfo() { press = tsdir != FPVector2.zero, dire = tsdir };
            
            gamepad.SetInput(InputType.Joystick, joystick);
            gamepad.SetInput(InputType.BA, new InputInfo() { press = bapress, dire = FPVector2.zero });
            gamepad.SetInput(InputType.BB, new InputInfo() { press = bbpress, dire = FPVector2.zero });
            gamepad.SetInput(InputType.BC, new InputInfo() { press = bcpress, dire = FPVector2.zero });
            gamepad.SetInput(InputType.BD, new InputInfo() { press = bdpress, dire = FPVector2.zero });
        }
    }
}
