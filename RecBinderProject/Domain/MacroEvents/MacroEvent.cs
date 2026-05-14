namespace RecBinderProject.Domain.MacroEvents
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

    public struct MacroEvent
    {
        public InputEventKind Kind { get; set; }
        public int TimeOffset { get; set; }

        public KeyboardPressEvent? KeyboardPress { get; set; }
        public MousePressEvent? MousePress { get; set; }
        public MouseMoveEvent? MouseMove { get; set; }
        public MouseWheelEvent? MouseWheel { get; set; }

        public static MacroEvent From(KeyboardPressEvent input) => new MacroEvent { Kind = InputEventKind.KeyboardPress, TimeOffset = input.TimeOffset, KeyboardPress = input };
        public static MacroEvent From(MousePressEvent input) => new MacroEvent { Kind = InputEventKind.MousePress, TimeOffset = input.TimeOffset, MousePress = input };
        public static MacroEvent From(MouseMoveEvent input) => new MacroEvent { Kind = InputEventKind.MouseMove, TimeOffset = input.TimeOffset, MouseMove = input };
        public static MacroEvent From(MouseWheelEvent input) => new MacroEvent { Kind = InputEventKind.MouseWheel, TimeOffset = input.TimeOffset, MouseWheel = input };
    }
}