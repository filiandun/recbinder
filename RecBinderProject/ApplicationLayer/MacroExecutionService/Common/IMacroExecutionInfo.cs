
namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Common
{
    public interface IMacroExecutionInfo
    {
        public event Action<MacroExecutionState>? StateChanged;

        public MacroExecutionState State { get; }

        public TimeSpan TotalDuration { get; }
        public TimeSpan ElapsedTime { get; }
    }
}
