using System.Windows.Forms;

namespace RecBinderProject
{
    public enum KeyState
    {
        Up,
        Down
    }

    public struct MouseClick
    {
        public MouseButtons MouseButton { get; set; } 
        public KeyState KeyState { get; set; }
        public int DelayMs { get; set; }

        public MouseClick(MouseButtons mouseButton, KeyState keyState, int delayMs)
        {
            this.MouseButton = mouseButton;
            this.KeyState = keyState;
            this.DelayMs = delayMs;
        }
    }
}
