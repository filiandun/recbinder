using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Managers
{
    public class MacroRecordingContext
    {
        private MacroMetadata? _macroMetadata;
        private MacroRecordingOptions? _recordingOptions;

        public MacroMetadata MacroMetadata => this._macroMetadata ?? throw new InvalidOperationException($"{nameof(MacroRecordingContext)} is not initialized: {nameof(this.MacroMetadata)} is null.");
        public MacroRecordingOptions RecordingOptions => this._recordingOptions ?? throw new InvalidOperationException($"{nameof(MacroRecordingContext)} is not initialized: {nameof(this.RecordingOptions)} is null.");

        public bool IsInitialized => this._macroMetadata is not null && this._recordingOptions is not null;

        public void Initialize(MacroMetadata macroMetadata, MacroRecordingOptions recordingOptions)
        {
            ArgumentNullException.ThrowIfNull(macroMetadata);
            ArgumentNullException.ThrowIfNull(recordingOptions);

            if (this.IsInitialized)
            {
                throw new InvalidOperationException($"{nameof(MacroRecordingContext)} is already initialized.");
            }

            this._macroMetadata = macroMetadata;
            this._recordingOptions = recordingOptions;
        }
    }
}