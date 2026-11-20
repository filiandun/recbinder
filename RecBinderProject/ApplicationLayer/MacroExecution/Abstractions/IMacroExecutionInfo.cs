using RecBinderProject.ApplicationLayer.MacroExecution.Common;


namespace RecBinderProject.ApplicationLayer.MacroExecution.Abstractions
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
