

namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Options
{
    public class MacroPlaybackOptions
    {
        public bool KeyboardPressIgnored { get; set; }
        public bool MousePressIgnored { get; set; }
        public bool MouseMoveIgnored { get; set; }
        public bool MouseWheelIgnored { get; set; }

        public bool RelativeMouseMovementEnabled { get; set; }


        public MacroPlaybackOptions()
        {
            this.KeyboardPressIgnored = false;
            this.MousePressIgnored = false;
            this.MouseMoveIgnored = false;
            this.MouseWheelIgnored = false;

            this.RelativeMouseMovementEnabled = false;
        }
    }
}
