using System.Windows.Forms;

using RecBinderProject.Domain.MacroEvents.Enums;


namespace RecBinderProject.Domain.MacroEvents
{
    public struct MousePressEvent
    {
        public MouseButtons MouseButton { get; set; }
        public MacroKeyState KeyState { get; set; }

        public MousePressEvent(MouseButtons mouseButton, MacroKeyState keyState)
        {
            this.MouseButton = mouseButton;
            this.KeyState = keyState;
        }
    }
}
