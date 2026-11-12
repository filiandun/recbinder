using System.ComponentModel;
using System.Runtime.CompilerServices;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;


namespace RecBinderProject.Presentation.ViewModels.MacroOptions
{
    public class MacroRecordingOptionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly MacroRecordingOptions _options;
        public MacroRecordingOptions Options => this._options;


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

        public bool TreeDimensionalMouseMovementEnabled
        {
            get => this._options.ThreeDimensionalMouseMovementEnabled;
            set
            {
                if (this._options.ThreeDimensionalMouseMovementEnabled == value) return;

                this._options.ThreeDimensionalMouseMovementEnabled = value;
                this.OnPropertyChanged();
            }
        }


        public MacroRecordingOptionsViewModel()
        {
            this._options = new MacroRecordingOptions();
        }


        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
