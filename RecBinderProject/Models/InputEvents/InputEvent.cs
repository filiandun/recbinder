namespace RecBinderProject.Models.InputEvents
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

        public KeyboardPressEvent? KeyboardPress { get; set; }
        public MousePressEvent? MousePress { get; set; }
        public MouseMoveEvent? MouseMove { get; set; }
        public MouseWheelEvent? MouseWheel { get; set; }

        public static InputEvent From(KeyboardPressEvent input) => new InputEvent { Kind = InputEventKind.KeyboardPress, TimeOffset = input.TimeOffset, KeyboardPress = input };
        public static InputEvent From(MousePressEvent input) => new InputEvent { Kind = InputEventKind.MousePress, TimeOffset = input.TimeOffset, MousePress = input };
        public static InputEvent From(MouseMoveEvent input) => new InputEvent { Kind = InputEventKind.MouseMove, TimeOffset = input.TimeOffset, MouseMove = input };
        public static InputEvent From(MouseWheelEvent input) => new InputEvent { Kind = InputEventKind.MouseWheel, TimeOffset = input.TimeOffset, MouseWheel = input };
    }
}
