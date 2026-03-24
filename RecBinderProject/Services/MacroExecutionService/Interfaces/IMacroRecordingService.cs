using RecBinderProject.Models;

namespace RecBinderProject.Services.MacroExecutionService.Interfaces
{
    public interface IMacroRecordingService : IMacroExecutionInfo
    {
        public void Start(MacroMetadata macroMetadata);
        public void Pause();
        public void Resume();
        public void Stop();
    }
}
