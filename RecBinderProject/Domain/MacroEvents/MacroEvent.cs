using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using RecBinderProject.Domain.MacroEvents.Enums;

using RecBinderProject.Infrastructure.Serialization;


namespace RecBinderProject.Domain.MacroEvents
{

    [JsonConverter(typeof(MacroEventJsonConverter))]
    [StructLayout(LayoutKind.Explicit)]
    public struct MacroEvent
    {
        [FieldOffset(0)]
        public MacroEventKind Kind;

        [FieldOffset(4)]
        public int TimeOffset;


        [FieldOffset(8)]
        private KeyboardPressEvent _keyboardPress;
        public KeyboardPressEvent KeyboardPress
        {
            get => this.Kind == MacroEventKind.KeyboardPress ? this._keyboardPress : throw new InvalidOperationException($"Cannot access {nameof(this.KeyboardPress)} on event of type {this.Kind}");
            private set => this._keyboardPress = value;
        }


        [FieldOffset(8)]
        private MousePressEvent _mousePress;
        public MousePressEvent MousePress
        {
            get => this.Kind == MacroEventKind.MousePress ? this._mousePress : throw new InvalidOperationException($"Cannot access {nameof(this.MousePress)} on event of type {this.Kind}");
            private set => this._mousePress = value;
        }


        [FieldOffset(8)]
        private MouseMoveEvent _mouseMove;
        public MouseMoveEvent MouseMove
        {
            get => this.Kind == MacroEventKind.MouseMove ? this._mouseMove : throw new InvalidOperationException($"Cannot access {nameof(this.MouseMove)} on event of type {this.Kind}");
            private set => this._mouseMove = value;
        }


        [FieldOffset(8)]
        private MouseWheelEvent _mouseWheel;
        public MouseWheelEvent MouseWheel 
        {
            get => this.Kind == MacroEventKind.MouseWheel ? this._mouseWheel : throw new InvalidOperationException($"Cannot access {nameof(this.MouseWheel)} on event of type {this.Kind}");
            private set => this._mouseWheel = value;
        }


        public static MacroEvent FromKeyboardPress(int keyCode, MacroKeyState keyState, int timeOffset) => new MacroEvent { Kind = MacroEventKind.KeyboardPress, TimeOffset = timeOffset, KeyboardPress = new KeyboardPressEvent(keyCode, keyState) };
        public static MacroEvent FromMousePress(MouseButtons mouseButton, MacroKeyState keyState, int timeOffset) => new MacroEvent { Kind = MacroEventKind.MousePress, TimeOffset = timeOffset, MousePress = new MousePressEvent(mouseButton, keyState) };
        public static MacroEvent FromMouseMove(double x, double y, int timeOffset) => new MacroEvent { Kind = MacroEventKind.MouseMove, TimeOffset = timeOffset, MouseMove = new MouseMoveEvent(x, y) };
        public static MacroEvent FromMouseWheel(int delta, int timeOffset) => new MacroEvent { Kind = MacroEventKind.MouseWheel, TimeOffset = timeOffset, MouseWheel = new MouseWheelEvent(delta) };
    }
}