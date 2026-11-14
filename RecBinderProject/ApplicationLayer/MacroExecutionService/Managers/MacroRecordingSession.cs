using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Managers
{
    public sealed record MacroRecordingSession : IDisposable
    {
        public MacroEventRecorder MacroRecorder { get; init; }

        public CancellationTokenSource CancellationTokenSource { get; init; }

        public MacroRecordingOptions RecordingOptions { get; init; }
        public Macro Macro { get; init; }


        public MacroRecordingSession(MacroEventRecorder macroRecorder, CancellationTokenSource cancellationTokenSource, MacroRecordingOptions recordingOptions, Macro macro)
        {
            this.MacroRecorder = macroRecorder;

            this.CancellationTokenSource = cancellationTokenSource;

            this.RecordingOptions = recordingOptions;
            this.Macro = macro;
        }


        public void Dispose()
        {
            this.CancellationTokenSource.Dispose();
            this.MacroRecorder.Dispose();
        }
    }
}
