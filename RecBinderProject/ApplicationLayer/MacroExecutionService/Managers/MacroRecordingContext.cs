using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Managers
{
    public class MacroRecordingContext
    {
        private Macro? _macro;
        private MacroRecordingOptions? _recordingOptions;

        public Macro Macro => this._macro ?? throw new InvalidOperationException($"{nameof(MacroRecordingContext)} is not initialized: {nameof(this.Macro)} is null.");
        public MacroRecordingOptions RecordingOptions => this._recordingOptions ?? throw new InvalidOperationException($"{nameof(MacroRecordingContext)} is not initialized: {nameof(this.RecordingOptions)} is null.");

        public bool IsInitialized => this._macro is not null && this._recordingOptions is not null;

        public void Initialize(Macro macro, MacroRecordingOptions recordingOptions)
        {
            ArgumentNullException.ThrowIfNull(macro);
            ArgumentNullException.ThrowIfNull(recordingOptions);

            if (this.IsInitialized)
            {
                throw new InvalidOperationException($"{nameof(MacroRecordingContext)} is already initialized.");
            }

            this._macro = macro;
            this._recordingOptions = recordingOptions;
        }
    }
}