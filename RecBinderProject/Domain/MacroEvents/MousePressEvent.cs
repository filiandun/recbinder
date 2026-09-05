using RecBinderProject.Domain.MacroEvents.Enums;


namespace RecBinderProject.Domain.MacroEvents
{
    public struct MousePressEvent
    {
        public int MouseButton { get; set; }
        public MacroKeyState KeyState { get; set; }

        public MousePressEvent(int mouseButton, MacroKeyState keyState)
        {
            this.MouseButton = mouseButton;
            this.KeyState = keyState;
        }
    }
}
