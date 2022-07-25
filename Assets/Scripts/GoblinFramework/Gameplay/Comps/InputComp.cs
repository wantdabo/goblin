using GoblinFramework.Core;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Comps
{
    public enum InputType
    {
        LInput,
        LJoystick,
        LBA,
        LBB,
        LBC,
        LBD,
    }

    public struct InputInfo
    {
        public bool press;
        public Fixed64Vector2 dire;
    }

    public class LInputComp : Comp<PGEngineComp>
    {
        private Dictionary<InputType, InputInfo> inputMap = new Dictionary<InputType, InputInfo>
        {
                {InputType.LJoystick, new InputInfo(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.LBA, new InputInfo(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.LBB, new InputInfo(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.LBC, new InputInfo(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.LBD, new InputInfo(){press = false, dire = Fixed64Vector2.Zero}},
        };

        public InputInfo GetInput(InputType inputType)
        {
            if (inputMap.TryGetValue(inputType, out InputInfo input)) return input;

            throw new Exception($"inputType not found {inputType}");
        }

        public void SetInput(InputType inputType, InputInfo input)
        {
            if (inputMap.ContainsKey(inputType)) inputMap.Remove(inputType);

            inputMap.Add(inputType, input);
        }
    }
}
