using RecBinderProject.ApplicationLayer.MacroExecution.Abstractions;
using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecution.Playback
{
    public sealed record MacroPlaybackSession : IDisposable
    {
        public IMacroEventPlayer MacroPlayer { get; init; }

        public CancellationTokenSource CancellationTokenSource { get; init; }

        public MacroPlaybackOptions PlaybackOptions { get; init; }
        public Macro Macro { get; init; }


        public MacroPlaybackSession(IMacroEventPlayer macroPlayer, CancellationTokenSource cancellationTokenSource, MacroPlaybackOptions playbackOptions, Macro macro)
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
