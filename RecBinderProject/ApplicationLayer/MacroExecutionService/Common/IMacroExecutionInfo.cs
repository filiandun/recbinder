namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Common
{
    public interface IMacroExecutionInfo
    {
        public event Action<MacroExecutionState>? StateChanged;

        public MacroExecutionState State { get; }

        public bool CanStart { get; }
        public bool CanPause { get; }
        public bool CanResume { get; }
        public bool CanStop { get; }

        public TimeSpan TotalDuration { get; }
        public TimeSpan ElapsedTime { get; }
    }
}
