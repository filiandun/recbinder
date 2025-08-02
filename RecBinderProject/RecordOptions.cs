
using System.ComponentModel;
using System.Windows;

namespace RecBinderProject
{
    public class RecordOptions : INotifyPropertyChanged
    {
        private bool _ignoreMouseMove = false;
        private bool _ignoreMouseWheel = false;
        private bool _ignoreMousePress = false;
        private bool _ignoreKeyboardPress = false;

        public bool IgnoreMouseMove
        {
            get => this._ignoreMouseMove;
            set
            {
                if (this._ignoreMouseMove != value)
                {
                    this._ignoreMouseMove = value;
                    this.OnPropertyChanged(nameof(this.IgnoreMouseMove));
                }
            }
        }

        public bool IgnoreMouseWheel
        {
            get => this._ignoreMouseWheel;
            set
            {
                if (this._ignoreMouseWheel != value)
                {
                    this._ignoreMouseWheel = value;
                    this.OnPropertyChanged(nameof(this.IgnoreMouseWheel));
                }
            }
        }

        public bool IgnoreMousePress
        {
            get => this._ignoreMousePress;
            set
            {
                if (this._ignoreMousePress != value)
                {
                    this._ignoreMousePress = value;
                    this.OnPropertyChanged(nameof(this.IgnoreMousePress));
                }
            }
        }

        public bool IgnoreKeyboardPress
        {
            get => this._ignoreKeyboardPress;
            set
            {
                if (this._ignoreKeyboardPress != value)
                {
                    this._ignoreKeyboardPress = value;
                    this.OnPropertyChanged(nameof(this.IgnoreKeyboardPress));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
