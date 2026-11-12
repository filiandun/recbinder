using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Playback
{
    public interface IMacroPlaybackService : IMacroExecutionInfo
    {
        public Task StartAsync(Macro macro, MacroPlaybackOptions playbackOptions);

        public void Pause();
        public void Resume();
        public void Stop();
    }
}