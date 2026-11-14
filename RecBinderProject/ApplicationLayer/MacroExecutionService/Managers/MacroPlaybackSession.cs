using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Managers
{
    public sealed record MacroPlaybackSession : IDisposable
    {
        public MacroEventPlayer MacroPlayer { get; init; }

        public CancellationTokenSource CancellationTokenSource { get; init; }

        public MacroPlaybackOptions PlaybackOptions { get; init; }
        public Macro Macro { get; init; }


        public MacroPlaybackSession(MacroEventPlayer macroPlayer, CancellationTokenSource cancellationTokenSource, MacroPlaybackOptions playbackOptions, Macro macro)
        {
            this.MacroPlayer = macroPlayer;

            this.CancellationTokenSource = cancellationTokenSource;

            this.PlaybackOptions = playbackOptions;
            this.Macro = macro;
        }


        public void Dispose()
        {
            this.CancellationTokenSource.Dispose();
        }
    }
}
