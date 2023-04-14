using System;
using System.Collections.Generic;
using GoblinFramework.Gameplay.Events;
using UnityEngine;

namespace GoblinFramework.Gameplay.Inputs
{
    public struct Input
    {
        public bool press;
        public bool release;
        public Vector2 dire;
    }

    public enum InputType
    {
        BA,
        BB,
        BC,
        BD,
        Joystick,
    }

    public class Gamepad : Behavior
    {
        private Dictionary<InputType, Input> InputMap = new()
        {
            { InputType.BA, new Input { press = false, dire = Vector2.zero } },
            { InputType.BB, new Input { press = false, dire = Vector2.zero } },
            { InputType.BC, new Input { press = false, dire = Vector2.zero } },
            { InputType.BD, new Input { press = false, dire = Vector2.zero } },
            { InputType.Joystick, new Input { press = false, dire = Vector2.zero } },
        };

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<LateTickEvent>(OnLateTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
        }

        public Input GetInput(InputType inputType)
        {
            if (InputMap.TryGetValue(inputType, out Input input)) return input;

            throw new Exception($"inputType not found {inputType}");
        }
        
        public void SetInput(InputType inputType, Input input)
        {
            if (InputMap.TryGetValue(inputType, out var oldInput))
            {
                // 上一次是 press，新的是 unpress 表示技能释放出去了。
                if (oldInput.press && false == input.press) input.release = true;
                InputMap.Remove(inputType);
            }

            InputMap.Add(inputType, input);
        }
        
        private void OnLateTick(LateTickEvent e)
        {
            ClearReleaseToken(InputType.BA);
            ClearReleaseToken(InputType.BB);
            ClearReleaseToken(InputType.BC);
            ClearReleaseToken(InputType.BD);
            ClearReleaseToken(InputType.Joystick);
        }

        private void ClearReleaseToken(InputType it)
        {
            var i = GetInput(InputType.BA);
            i.release = false;
            InputMap.Remove(it);
            InputMap.Add(it, i);
        }
    }
}