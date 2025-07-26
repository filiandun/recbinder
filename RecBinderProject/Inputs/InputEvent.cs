using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecBinderProject.Inputs
{
    public enum KeyState
    {
        Up,
        Down
    }

    public enum InputEventKind
    {
        KeyboardPress,
        MousePress,
        MouseMove,
        MouseWheel
    }

    public struct InputEvent
    {
        public InputEventKind Kind { get; set; }
        public int TimeOffset { get; set; }

        public KeyboardPressInput? KeyboardPress { get; set; }
        public MousePressInput? MousePress { get; set; }
        public MouseMoveInput? MouseMove { get; set; }
        public MouseWheelInput? MouseWheel { get; set; }

        public static InputEvent From(KeyboardPressInput input) => new InputEvent { Kind = InputEventKind.KeyboardPress, TimeOffset = input.TimeOffset, KeyboardPress = input };
        public static InputEvent From(MousePressInput input) => new InputEvent { Kind = InputEventKind.MousePress, TimeOffset = input.TimeOffset, MousePress = input };
        public static InputEvent From(MouseMoveInput input) => new InputEvent { Kind = InputEventKind.MouseMove, TimeOffset = input.TimeOffset, MouseMove = input };
        public static InputEvent From(MouseWheelInput input) => new InputEvent { Kind = InputEventKind.MouseWheel, TimeOffset = input.TimeOffset, MouseWheel = input };
    }
}
