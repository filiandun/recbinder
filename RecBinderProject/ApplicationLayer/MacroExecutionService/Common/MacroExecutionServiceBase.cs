
namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Common
{
    public abstract class MacroExecutionServiceBase : IMacroExecutionInfo
    {
        public event Action<MacroExecutionState>? StateChanged;

        private MacroExecutionState _state;
        public MacroExecutionState State
        {
            get => _state;
            protected set => SetState(value);
        }

        public abstract TimeSpan TotalDuration { get; }
        public abstract TimeSpan ElapsedTime { get; }

        protected bool CanStart() => this.State != MacroExecutionState.Playing;
        protected bool CanPause() => this.State == MacroExecutionState.Playing;
        protected bool CanResume() => this.State == MacroExecutionState.Paused;
        protected bool CanStop() => this.State == MacroExecutionState.Playing || this.State == MacroExecutionState.Paused;

        private void SetState(MacroExecutionState newState)
        {
            if (this._state != newState)
            {
                this._state = newState;
                this.StateChanged?.Invoke(this._state);
            }
        }

        public MacroExecutionServiceBase()
        {
            this._state = MacroExecutionState.Completed;
        }
    }
}
