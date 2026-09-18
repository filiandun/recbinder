

namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Options
{
    public class MacroPlaybackOptions
    {
        public const double DEFAULT_SPEED = 1.0;

        public const int DEFAULT_LOOP_COUNT = 1;
        public const int DEFAULT_LOOP_DELAY = 0;


        public required bool KeyboardPressIgnored { get; set; }
        public required bool MousePressIgnored { get; set; }
        public required bool MouseMoveIgnored { get; set; }
        public required bool MouseWheelIgnored { get; set; }

        public required bool RelativeMouseMovementEnabled { get; set; } 
        public required bool TrimPlaybackEndEnabled { get; set; }

        public required double Speed { get; set; }

        public required bool LoopEnabled { get; set; }
        public required int LoopCount { get; set; }
        public required int LoopDelayMs { get; set; }


        public MacroPlaybackOptions()
        {
            this.KeyboardPressIgnored = false;
            this.MousePressIgnored = false;
            this.MouseMoveIgnored = false;
            this.MouseWheelIgnored = false;

            this.RelativeMouseMovementEnabled = false;
            this.TrimPlaybackEndEnabled = false;

            this.Speed = DEFAULT_SPEED;

            this.LoopEnabled = false;
            this.LoopCount = DEFAULT_LOOP_COUNT;
            this.LoopDelayMs = DEFAULT_LOOP_DELAY;
        }
    }
}
