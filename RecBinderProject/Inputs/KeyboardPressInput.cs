using RecBinderProject.Inputs;

namespace RecBinderProject
{
    public struct KeyboardPressInput
    {
        public int KeyCode { get; set; }
        public KeyState KeyState { get; set; }
        public int TimeOffset { get; set; }

        public KeyboardPressInput(int keyCode, KeyState keyState, int timeOffset)
        {
            this.KeyCode = keyCode;
            this.KeyState = keyState;
            this.TimeOffset = timeOffset;
        }
    }
}
