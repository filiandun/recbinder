

namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Options
{
    public class MacroRecordingOptions
    {
        public bool KeyboardPressIgnored { get; set; }
        public bool MousePressIgnored { get; set; }
        public bool MouseMoveIgnored { get; set; }
        public bool MouseWheelIgnored { get; set; }

        
        public MacroRecordingOptions()
        {
            this.KeyboardPressIgnored = false;
            this.MousePressIgnored = false;
            this.MouseMoveIgnored = false;
            this.MouseWheelIgnored = false;
        }
    }
}
