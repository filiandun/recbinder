using RecBinderProject.Domain.MacroEvents.Enums;


namespace RecBinderProject.Domain.MacroEvents
{
    public struct KeyboardPressEvent
    {
        public int KeyCode { get; set; }
        public MacroKeyState KeyState { get; set; }

        public KeyboardPressEvent(int keyCode, MacroKeyState keyState)
        {
            this.KeyCode = keyCode;
            this.KeyState = keyState;
        }
    }
}
