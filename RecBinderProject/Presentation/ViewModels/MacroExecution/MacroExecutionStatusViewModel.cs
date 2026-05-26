using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Threading;
using System.Runtime.CompilerServices;

using RecBinderProject.Infrastructure.LocalizationService;

using RecBinderProject.ApplicationLayer.MacroExecutionService;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Common;


namespace RecBinderProject.Presentation.ViewModels.MacroExecution
{
    public class MacroExecutionStatusViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly DispatcherTimer _timer;

        private readonly ILocalizationService _localizationService;
        private readonly string _localizationPrefix;

        private readonly IMacroExecutionInfo _recordInfo;

        private string _statusText;

        private Brush _progressBarColor;
        private double _progressBarValue;


        public string StatusText
        {
            get => this._statusText;
            private set => this.Set(ref this._statusText, value);
        }

        public Brush ProgressBarColor
        {
            get => this._progressBarColor;
            private set => this.Set(ref this._progressBarColor, value);
        }

        public double ProgressBarValue
        {
            get => this._progressBarValue;
            private set => this.Set(ref this._progressBarValue, value);
        }


        public MacroExecutionStatusViewModel(IMacroExecutionInfo recordService, ILocalizationService localizationService, string localizationPrefix)
        {
            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromMilliseconds(33);
            this._timer.Tick += OnTimerTick;

            this._localizationService = localizationService;
            this._localizationPrefix = localizationPrefix;

            this._recordInfo = recordService;
            this._recordInfo.StateChanged += this.Service_StateChanged;

            this._statusText = this.GetLocalization("ButtonHint");

            this._progressBarColor = new SolidColorBrush(Colors.CornflowerBlue);
            this._progressBarValue = 0;
        }


        public void SetPlayingState()
        {
            this._timer.Start();

            this.StatusText = $"{this.GetLocalization("Playing")} - {this.GetTimeString()}";
            this.ProgressBarColor = new SolidColorBrush(Colors.CornflowerBlue); // TODO убрать инициализацию кистей тут и ниже (сделать static readonly поля для них, может даже в отдельном классе)
        }

        public void SetPausedState()
        {
            this._timer.Stop();

            this.StatusText = $"{this.GetLocalization("Paused")} - {this.GetTimeString()}";
            this.ProgressBarColor = new SolidColorBrush(Colors.Orange);
        }

        public void SetCompletedState()
        {
            this._timer.Stop();

            this.StatusText = $"{this.GetLocalization("Completed")} - {this.GetTimeString()}";
            this.ProgressBarValue = 100;
            this.ProgressBarColor = new SolidColorBrush(Colors.LimeGreen);
        }

        public void SetStoppedState()
        {
            this._timer.Stop();

            this.StatusText = $"{this.GetLocalization("Stopped")} - {this.GetTimeString()}";
            this.ProgressBarColor = new SolidColorBrush(Colors.Red);
        }

        public void Dispose()
        {
            this._timer.Stop();
            this._recordInfo.StateChanged -= this.Service_StateChanged;
        }


        private void Service_StateChanged(MacroExecutionState newProcessState)
        {
            switch (newProcessState)
            {
                case MacroExecutionState.Playing: this.SetPlayingState(); break;
                case MacroExecutionState.Paused: this.SetPausedState(); break;
                case MacroExecutionState.Completed: this.SetCompletedState(); break;
                case MacroExecutionState.Stopped: this.SetStoppedState(); break;
            }
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            this.StatusText = $"{this.GetLocalization("Playing")} - {this.GetTimeString()}";
            this.UpdateProgressBar();
        }

        private string GetTimeString() => $"{this._recordInfo.ElapsedTime.ToString(@"hh\:mm\:ss\.ff")} / {this._recordInfo.TotalTime.ToString(@"hh\:mm\:ss\.ff")}";

        private string GetLocalization(string key) => this._localizationService.GetString(this._localizationPrefix + "_" + key);

        private void UpdateProgressBar()
        {
            TimeSpan duration = this._recordInfo.TotalTime;
            TimeSpan elapsed = this._recordInfo.ElapsedTime;

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