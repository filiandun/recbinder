
namespace RecBinderProject.ApplicationLayer.MacroExecution.Playback
{
    public class MacroPlaybackOptions
    {
        public const double DEFAULT_SPEED = 1.0;

        public const int DEFAULT_LOOP_COUNT = 1;
        public const int DEFAULT_LOOP_DELAY = 0;


        public bool KeyboardPressIgnored { get; set; } = false;
        public bool MousePressIgnored { get; set; } = false;
        public bool MouseMoveIgnored { get; set; } = false;
        public bool MouseWheelIgnored { get; set; } = false;

        public bool RelativeMouseMovementEnabled { get; set; } = false;
        public bool TrimPlaybackEndEnabled { get; set; } = false;

        public double Speed { get; set; } = DEFAULT_SPEED;

        public bool LoopEnabled { get; set; } = false;
        public int LoopCount { get; set; } = DEFAULT_LOOP_COUNT;
        public int LoopDelayMs { get; set; } = DEFAULT_LOOP_DELAY;
    }
}
