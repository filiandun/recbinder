using System.Windows.Forms;

namespace RecBinderProject.Domain.MacroEvents
{
    public struct MousePressEvent
    {
        public MouseButtons MouseButton { get; set; } 
        public KeyState KeyState { get; set; }
        public int TimeOffset { get; set; }

        public MousePressEvent(MouseButtons mouseButton, KeyState keyState, int timeOffset)
        {
            this.MouseButton = mouseButton;
            this.KeyState = keyState;
            this.TimeOffset = timeOffset;
        }
    }
}
