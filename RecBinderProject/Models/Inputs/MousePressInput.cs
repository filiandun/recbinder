using System.Windows.Forms;

namespace RecBinderProject.Models.Inputs
{
    public struct MousePressInput
    {
        public MouseButtons MouseButton { get; set; } 
        public KeyState KeyState { get; set; }
        public int TimeOffset { get; set; }

        public MousePressInput(MouseButtons mouseButton, KeyState keyState, int timeOffset)
        {
            MouseButton = mouseButton;
            KeyState = keyState;
            TimeOffset = timeOffset;
        }
    }
}
