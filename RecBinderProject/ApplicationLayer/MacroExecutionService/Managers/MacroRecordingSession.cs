using Microsoft.Extensions.DependencyInjection;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Managers
{
    public sealed record MacroRecordingSession : IDisposable
    {
        public IServiceScope ServiceScope { get; init; }
        public MacroEventRecorder MacroRecorder { get; init; }

        public CancellationTokenSource CancellationTokenSource { get; init; }

        public Macro Macro { get; init; }


        public MacroRecordingSession(IServiceScope serviceScope, MacroEventRecorder macroRecorder, CancellationTokenSource cancellationTokenSource, Macro macro)
        {
            this.ServiceScope = serviceScope;
            this.MacroRecorder = macroRecorder;

            this.CancellationTokenSource = cancellationTokenSource;

            this.Macro = macro;
        }


        public void Dispose()
        {
            this.CancellationTokenSource.Dispose();
            this.ServiceScope.Dispose();
        }
    }
}
