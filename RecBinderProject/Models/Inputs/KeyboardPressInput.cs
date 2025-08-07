namespace RecBinderProject.Models.Inputs
{
    public struct KeyboardPressInput
    {
        public int KeyCode { get; set; }
        public KeyState KeyState { get; set; }
        public int TimeOffset { get; set; }

        public KeyboardPressInput(int keyCode, KeyState keyState, int timeOffset)
        {
            KeyCode = keyCode;
            KeyState = keyState;
            TimeOffset = timeOffset;
        }
    }
}
