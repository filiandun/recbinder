using RecBinderProject.ApplicationLayer.MacroExecution.Abstractions;
using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecution.Recording
{
    public sealed record MacroRecordingSession : IDisposable
    {
        public IMacroEventRecorder MacroRecorder { get; init; }

        public CancellationTokenSource CancellationTokenSource { get; init; }

        public MacroRecordingOptions RecordingOptions { get; init; }
        public Macro Macro { get; init; }


        public MacroRecordingSession(IMacroEventRecorder macroRecorder, CancellationTokenSource cancellationTokenSource, MacroRecordingOptions recordingOptions, Macro macro)
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
