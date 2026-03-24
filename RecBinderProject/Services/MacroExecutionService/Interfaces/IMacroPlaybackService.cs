
namespace RecBinderProject.Services.MacroExecutionService.Interfaces
{
    public interface IMacroPlaybackService : IMacroExecutionInfo
    {
        public void Start();
        public void Pause();
        public void Resume();
        public void Stop();
    }
}