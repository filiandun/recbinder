using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Recording
{
    public interface IMacroRecordingService : IMacroExecutionInfo
    {
        public bool CanSave { get; }

        public Task StartAsync(MacroMetadata macroMetadata, MacroRecordingOptions recordingOptions);
        public void Pause();
        public void Resume();
        public void Stop();

        public Task SaveMacroAsync();
    }
}
