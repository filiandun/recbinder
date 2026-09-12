

namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Options
{
    public class MacroPlaybackOptions
    {
        public required bool KeyboardPressIgnored { get; set; }
        public required bool MousePressIgnored { get; set; }
        public required bool MouseMoveIgnored { get; set; }
        public required bool MouseWheelIgnored { get; set; }

        public required bool RelativeMouseMovementEnabled { get; set; } 
        public required bool TrimPlaybackEndEnabled { get; set; }


        public MacroPlaybackOptions()
        {
            this.KeyboardPressIgnored = false;
            this.MousePressIgnored = false;
            this.MouseMoveIgnored = false;
            this.MouseWheelIgnored = false;

            this.RelativeMouseMovementEnabled = false;
            this.TrimPlaybackEndEnabled = false;
        }
    }
}
