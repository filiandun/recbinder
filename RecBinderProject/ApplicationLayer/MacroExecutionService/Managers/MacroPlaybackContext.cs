using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Managers
{
    public class MacroPlaybackContext
    {
        private Macro? _macro;
        private MacroPlaybackOptions? _playbackOptions;

        public Macro Macro => this._macro ?? throw new InvalidOperationException($"{nameof(MacroPlaybackContext)} is not initialized: {nameof(this.Macro)} is null.");
        public MacroPlaybackOptions PlaybackOptions => this._playbackOptions ?? throw new InvalidOperationException($"{nameof(MacroPlaybackContext)} is not initialized: {nameof(this.PlaybackOptions)} is null.");

        public bool IsInitialized => this._macro is not null && this._playbackOptions is not null;

        public void Initialize(Macro macro, MacroPlaybackOptions playbackOptions)
        {
            ArgumentNullException.ThrowIfNull(macro);
            ArgumentNullException.ThrowIfNull(playbackOptions);

            if (this.IsInitialized)
            {
                throw new InvalidOperationException($"{nameof(MacroPlaybackContext)} is already initialized.");
            }

            this._macro = macro;
            this._playbackOptions = playbackOptions;
        }
    }
}