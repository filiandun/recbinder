using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService
{
    public class MacroExecutionOptions : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _keyboardPressIgnored = false;
        private bool _mousePressIgnored = false;
        private bool _mouseMoveIgnored = false;
        private bool _mouseWheelIgnored = false;

        private bool _relativeMouseMovementEnabled = false;


        public bool KeyboardPressIgnored
        {
            get => this._keyboardPressIgnored;
            set => this.Set(ref this._keyboardPressIgnored, value);
        }

        public bool MousePressIgnored
        {
            get => this._mousePressIgnored;
            set => this.Set(ref this._mousePressIgnored, value);
        }

        public bool MouseMoveIgnored
        {
            get => this._mouseMoveIgnored;
            set => this.Set(ref this._mouseMoveIgnored, value);
        }

        public bool MouseWheelIgnored
        {
            get => this._mouseWheelIgnored;
            set => this.Set(ref this._mouseWheelIgnored, value);
        }


        public bool RelativeMouseMovementEnabled
        {
            get => this._relativeMouseMovementEnabled;
            set => this.Set(ref this._relativeMouseMovementEnabled, value);
        }


        private void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            Debug.WriteLine($"Record option changed - {propName}");
            if (!Equals(field, value))
            {
                field = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
