
namespace RecBinderProject.Services.MacroExecutionService.Interfaces
{
    public interface IMacroExecutionInfo
    {
        public event Action<MacroExecutionState>? StateChanged;

        public MacroExecutionState State { get; }

        public TimeSpan TotalTime { get; }
        public TimeSpan ElapsedTime { get; }
    }
}
