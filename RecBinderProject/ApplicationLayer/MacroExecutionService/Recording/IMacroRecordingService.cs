using RecBinderProject.Domain;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Recording
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
