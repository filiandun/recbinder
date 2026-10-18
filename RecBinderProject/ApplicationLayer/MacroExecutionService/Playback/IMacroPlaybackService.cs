using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Playback
{
    public interface IMacroPlaybackService : IMacroExecutionInfo
    {
        public Task StartAsync();

        public void Pause();
        public void Resume();
        public void Stop();
    }
}