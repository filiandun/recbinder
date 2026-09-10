

namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Options
{
    public class MacroRecordingOptions
    {
        public required bool KeyboardPressIgnored { get; set; }
        public required bool MousePressIgnored { get; set; }
        public required bool MouseMoveIgnored { get; set; }
        public required bool MouseWheelIgnored { get; set; }

        public required bool TreeDimensionalMouseMovementEnabled { get; set; }

        public MacroRecordingOptions()
        {
            this.KeyboardPressIgnored = false;
            this.MousePressIgnored = false;
            this.MouseMoveIgnored = false;
            this.MouseWheelIgnored = false;

            this.TreeDimensionalMouseMovementEnabled = false;
        }
    }
}
