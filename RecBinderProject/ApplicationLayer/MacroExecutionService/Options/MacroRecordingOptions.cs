

namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Options
{
    public class MacroRecordingOptions
    {
        public bool KeyboardPressIgnored { get; set; } = false;
        public bool MousePressIgnored { get; set; } = false;
        public bool MouseMoveIgnored { get; set; } = false;
        public bool MouseWheelIgnored { get; set; } = false;

        public bool ThreeDimensionalMouseMovementEnabled { get; set; } = false;
    }
}
