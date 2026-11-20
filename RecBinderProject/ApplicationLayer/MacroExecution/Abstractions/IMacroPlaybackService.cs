using RecBinderProject.ApplicationLayer.MacroExecution.Playback;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecution.Abstractions
{
    public interface IMacroPlaybackService : IMacroExecutionInfo
    {
        public Task StartAsync(Macro macro, MacroPlaybackOptions playbackOptions);

        public void Pause();
        public void Resume();
        public void Stop();
    }
}