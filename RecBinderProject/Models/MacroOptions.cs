using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace RecBinderProject.Models
{
    public class MacroOptions : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _keyboardPressEnabled = false;
        private bool _mousePressEnabled = false;
        private bool _mouseMoveEnabled = false;
        private bool _mouseWheelEnabled = false;

        private bool _relativeMouseMovementEnabled = false;

        public bool KeyboardPressEnabled
        {
            get => this._keyboardPressEnabled;
            set => this.Set(ref this._keyboardPressEnabled, value);
        }

        public bool MousePressEnabled
        {
            get => this._mousePressEnabled;
            set => this.Set(ref this._mousePressEnabled, value);
        }

        public bool MouseMoveEnabled
        {
            get => this._mouseMoveEnabled;
            set => this.Set(ref this._mouseMoveEnabled, value);
        }

        public bool MouseWheelEnabled
        {
            get => this._mouseWheelEnabled;
            set => this.Set(ref this._mouseWheelEnabled, value);
        }

        public bool RelativeMouseMovementEnabled
        {
            get => this._relativeMouseMovementEnabled;
            set => this.Set(ref this._relativeMouseMovementEnabled, value);
        }

        protected void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
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
