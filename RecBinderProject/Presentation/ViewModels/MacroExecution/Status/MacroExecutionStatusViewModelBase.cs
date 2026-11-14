using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Threading;
using System.Runtime.CompilerServices;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;
using RecBinderProject.ApplicationLayer.Localization;


namespace RecBinderProject.Presentation.ViewModels.MacroExecution.Status
{
    public abstract class MacroExecutionStatusViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly DispatcherTimer _timer;

        private readonly ILocalizationService _localizationService;
        private readonly IMacroExecutionInfo _executionInfo;

        private static readonly SolidColorBrush ProcessingColor = new SolidColorBrush(Colors.CornflowerBlue);
        private static readonly SolidColorBrush PausedColor = new SolidColorBrush(Colors.Orange);
        private static readonly SolidColorBrush CompletedColor = new SolidColorBrush(Colors.LimeGreen);
        private static readonly SolidColorBrush StoppedColor = new SolidColorBrush(Colors.Red);

        protected abstract string HintLocalizationKey { get; }
        protected abstract string ProcessingLocalizationKey { get; }
        protected abstract string PausedLocalizationKey { get; }
        protected abstract string CompletedLocalizationKey { get; }
        protected abstract string StoppedLocalizationKey { get; }


        private string _statusText;
        public string StatusText
        {
            get => this._statusText;
            private set => this.Set(ref this._statusText, value);
        }
        private double _progressBarValue;
        public double ProgressBarValue
        {
            get => this._progressBarValue;
            private set => this.Set(ref this._progressBarValue, value);
        }

        private Brush _progressBarColor;
        public Brush ProgressBarColor
        {
            get => this._progressBarColor;
            private set => this.Set(ref this._progressBarColor, value);
        }


        public MacroExecutionStatusViewModelBase(IMacroExecutionInfo executionService, ILocalizationService localizationService)
        {
            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(33);
            this._timer.Tick += OnTimerTick;

            this._localizationService = localizationService;

            this._executionInfo = executionService;
            this._executionInfo.StateChanged += this.OnStateChanged;

            this._statusText = this.GetLocalization(this.HintLocalizationKey);

            this._progressBarValue = 0;
            this._progressBarColor = ProcessingColor;
        }


        public void SetProcessingState()
        {
            this._timer.Start();

            this.StatusText = this.GetLocalizationWithTime(this.ProcessingLocalizationKey);
            this.ProgressBarColor = ProcessingColor;
        }

        public void SetPausedState()
        {
            this._timer.Stop();

            this.StatusText = this.GetLocalizationWithTime(this.PausedLocalizationKey);
            this.ProgressBarColor = PausedColor;
        }

        public void SetCompletedState()
        {
            this._timer.Stop();

            this.StatusText = this.GetLocalizationWithTime(this.CompletedLocalizationKey);
            this.ProgressBarValue = 100;
            this.ProgressBarColor = CompletedColor;
        }

        public void SetStoppedState()
        {
            this._timer.Stop();

            this.StatusText = this.GetLocalizationWithTime(this.StoppedLocalizationKey);
            this.ProgressBarColor = StoppedColor;
        }


        public void Dispose()
        {
            this._timer.Stop();
            this._executionInfo.StateChanged -= this.OnStateChanged;
        }


        private void OnStateChanged(MacroExecutionState newProcessState)
        {
            switch (newProcessState)
            {
                case MacroExecutionState.Executing: this.SetProcessingState(); break;
                case MacroExecutionState.Paused: this.SetPausedState(); break;
                case MacroExecutionState.Completed: this.SetCompletedState(); break;
                case MacroExecutionState.Stopped: this.SetStoppedState(); break;
            }
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            this.StatusText = this.GetLocalizationWithTime(this.ProcessingLocalizationKey);
            this.UpdateProgressBar();
        }

        private string GetTimeString() => $"{this._executionInfo.ElapsedTime.ToString(@"hh\:mm\:ss\.ff")} / {this._executionInfo.TotalDuration.ToString(@"hh\:mm\:ss\.ff")}";
        private string GetLocalization(string key) => this._localizationService.GetString(LocalizationScope.MacroExecution, key);
        private string GetLocalizationWithTime(string key) => $"{this.GetLocalization(key)} - {this.GetTimeString()}";

        private void UpdateProgressBar()
        {
            TimeSpan duration = this._executionInfo.TotalDuration;
            TimeSpan elapsed = this._executionInfo.ElapsedTime;

            this.ProgressBarValue = duration > TimeSpan.Zero ? (elapsed.TotalMilliseconds * 100) / duration.TotalMilliseconds : 0;
        }


        private void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}