using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Inputs;
using TrueSync;
using UnityEngine;

namespace Goblin.Sys.Gameplay
{
    /// <summary>
    /// 输入系统
    /// </summary>
    public class InputSystem : Comp
    {
        public Vector2 joystickdire { get; set; }
        public bool bapress { get; set; }
        public bool bbpress { get; set; }
        public bool bcpress { get; set; }
        public bool bdpress { get; set; }

        public void Input(uint actorId, Stage stage)
        {
            var player = stage.GetActor(actorId);
            var gamepad = player.GetBehavior<Gamepad>();
            
            var dir = joystickdire * Config.float2Int;
            var tsdir = new TSVector2() { x = Mathf.CeilToInt(dir.x) * FP.EN3, y = FP.Zero };
            var joystick = new InputInfo() { press = tsdir != TSVector2.zero, dire = tsdir };
            
            gamepad.SetInput(InputType.Joystick, joystick);
            gamepad.SetInput(InputType.BA, new InputInfo() { press = bapress, dire = TSVector2.zero });
            gamepad.SetInput(InputType.BB, new InputInfo() { press = bbpress, dire = TSVector2.zero });
            gamepad.SetInput(InputType.BC, new InputInfo() { press = bcpress, dire = TSVector2.zero });
            gamepad.SetInput(InputType.BD, new InputInfo() { press = bdpress, dire = TSVector2.zero });
        }
    }
}
