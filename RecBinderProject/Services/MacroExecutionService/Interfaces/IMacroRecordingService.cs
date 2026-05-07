using RecBinderProject.Models;

namespace RecBinderProject.Services.MacroExecutionService.Interfaces
{
    public interface IMacroRecordingService : IMacroExecutionInfo
    {
        public event Action? MacroSaved;

        public void Start(MacroMetadata macroMetadata);
        public void Pause();
        public void Resume();
        public void Stop();
    }
}
