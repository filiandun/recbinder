using System.ComponentModel;
using System.Runtime.CompilerServices;

using RecBinderProject.ApplicationLayer.MacroExecution.Playback;


namespace RecBinderProject.Presentation.ViewModels.MacroOptions
{
    public class MacroPlaybackOptionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly MacroPlaybackOptions _options;
        public MacroPlaybackOptions Options => this._options;


        public bool KeyboardPressIgnored
        {
            get => this._options.KeyboardPressIgnored;
            set
            {
                if (this._options.KeyboardPressIgnored == value) return;

                this._options.KeyboardPressIgnored = value;
                this.OnPropertyChanged();
            }
        }

        public bool MousePressIgnored
        {
            get => this._options.MousePressIgnored;
            set
            {
                if (this._options.MousePressIgnored == value) return;

                this._options.MousePressIgnored = value;
                this.OnPropertyChanged();
            }
        }

        public bool MouseMoveIgnored
        {
            get => this._options.MouseMoveIgnored;
            set
            {
                if (this._options.MouseMoveIgnored == value) return;

                this._options.MouseMoveIgnored = value;
                this.OnPropertyChanged();
            }
        }

        public bool MouseWheelIgnored
        {
            get => this._options.MouseWheelIgnored;
            set
            {
                if (this._options.MouseWheelIgnored == value) return;

                this._options.MouseWheelIgnored = value;
                this.OnPropertyChanged();
            }
        }

        public bool RelativeMouseMovementEnabled
        {
            get => this._options.RelativeMouseMovementEnabled;
            set
            {
                if (this._options.RelativeMouseMovementEnabled == value) return;

                this._options.RelativeMouseMovementEnabled = value;
                this.OnPropertyChanged();
            }
        }


        public bool TrimPlaybackEndEnabled
        {
            get => this._options.TrimPlaybackEndEnabled;
            set
            {
                if (this._options.TrimPlaybackEndEnabled == value) return;

                this._options.TrimPlaybackEndEnabled = value;
                this.OnPropertyChanged();
            }
        }

        // TODO валидация для полей
        public double Speed
        {
            get => this._options.Speed;
            set
            {
                if (this._options.Speed == value) return;

                this._options.Speed = value;
                this.OnPropertyChanged();
            }
        }

        public bool LoopEnabled
        {
            get => this._options.LoopEnabled;
            set
            {
                if (this._options.LoopEnabled == value) return;

                this._options.LoopEnabled = value;
                this.OnPropertyChanged();
            }
        }

        public int LoopCount
        {
            get => this._options.LoopCount;
            set
            {
                if (this._options.LoopCount == value) return;

                this._options.LoopCount = value;
                this.OnPropertyChanged();
            }
        }

        public int LoopDelayMs
        {
            get => this._options.LoopDelayMs;
            set
            {
                if (this._options.LoopDelayMs == value) return;

                this._options.LoopDelayMs = value;
                this.OnPropertyChanged();
            }
        }


        public MacroPlaybackOptionsViewModel()
        {
            this._options = new MacroPlaybackOptions();
        }


        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            System.Diagnostics.Debug.WriteLine($"Property '{propertyName}' was changed");
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
