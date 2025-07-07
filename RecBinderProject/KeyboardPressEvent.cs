namespace RecBinderProject
{
    public struct KeyboardPressEvent : IInputEvent
    {
        public int KeyCode { get; set; }
        public KeyState KeyState { get; set; }
        public int TimeOffset { get; set; }

        public KeyboardPressEvent(int keyCode, KeyState keyState, int timeOffset)
        {
            this.KeyCode = keyCode;
            this.KeyState = keyState;
            this.TimeOffset = timeOffset;
        }
    }
}
