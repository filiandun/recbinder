using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;
using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Managers
{
    public sealed record MacroPlaybackSession : IDisposable
    {
        public IServiceScope ServiceScope { get; init; }
        public MacroEventPlayer MacroPlayer { get; init; }

        public CancellationTokenSource CancellationTokenSource { get; init; }

        public MacroPlaybackOptions PlaybackOptions { get; init; }
        public Macro Macro { get; init; }


        public MacroPlaybackSession(IServiceScope serviceScope, MacroEventPlayer macroPlayer, CancellationTokenSource cancellationTokenSource, MacroPlaybackOptions playbackOptions, Macro macro)
        {
            this.ServiceScope = serviceScope;
            this.MacroPlayer = macroPlayer;

            this.CancellationTokenSource = cancellationTokenSource;

            this.PlaybackOptions = playbackOptions;
            this.Macro = macro;
        }


        public void Dispose()
        {
            this.CancellationTokenSource.Dispose();
            this.ServiceScope.Dispose();
        }
    }
}
