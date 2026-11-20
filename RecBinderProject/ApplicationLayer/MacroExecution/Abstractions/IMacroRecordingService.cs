using RecBinderProject.ApplicationLayer.MacroExecution.Recording;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecution.Abstractions
{
    public interface IMacroRecordingService : IMacroExecutionInfo, IDisposable
    {
        public bool CanSave { get; }

        public Task StartAsync(MacroMetadata macroMetadata, MacroRecordingOptions recordingOptions);
        public void Pause();
        public void Resume();
        public void Stop();

        public Task SaveMacroAsync();
    }
}
