using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace RecBinderProject.ViewModels.RecordingMetadataViewModels
{
    public class RecordingStateViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string ButtonText { get; private set; }

        public Brush ProgressBarColor { get; private set; }
        public double ProgressBarValue { get; private set; }

        public RecordingStateViewModel()
        {
            this.ButtonText = "Нажмите для воспроизведения";

            this.ProgressBarColor = new SolidColorBrush(Colors.Green);
            this.ProgressBarValue = 0;
        }


        public void SetRecordingState(TimeSpan elapsed, TimeSpan duration)
        {
            this.ButtonText = $"Запись воспроизводится - {this.GetTimeString(elapsed, duration)}";
            this.ProgressBarValue = (elapsed.TotalMilliseconds * 100) / duration.TotalMilliseconds;

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProgressBarValue)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ButtonText)));
        }

        public void SetPausedState(TimeSpan elapsed, TimeSpan duration)
        {
            this.ButtonText = $"Воспроизведение приостановлено - {this.GetTimeString(elapsed, duration)}";
            this.ProgressBarColor = new SolidColorBrush(Colors.Orange);

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProgressBarValue)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ButtonText)));
        }

        public void SetStoppedState(TimeSpan elapsed, TimeSpan duration)
        {
            this.ButtonText = $"Воспроизведение завершилось - {this.GetTimeString(elapsed, duration)}";
            this.ProgressBarColor = new SolidColorBrush(Colors.Orange);

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProgressBarValue)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ButtonText)));
        }

        public void SetHandleStoppedState(TimeSpan elapsed, TimeSpan duration)
        {
            this.ButtonText = $"Воспроизведение остановлено - {this.GetTimeString(elapsed, duration)}";
            this.ProgressBarColor = new SolidColorBrush(Colors.Red);

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ProgressBarValue)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ButtonText)));
        }

        private string GetTimeString(TimeSpan elapsed, TimeSpan duration) => $"{elapsed.ToString(@"hh\:mm\:ss\.ff")} / {duration.ToString(@"hh\:mm\:ss\.ff")}";

        //private void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
        //{
        //    if (!Equals(field, value))
        //    {
        //        field = value;
        //        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        //    }
        //}
    }
}
