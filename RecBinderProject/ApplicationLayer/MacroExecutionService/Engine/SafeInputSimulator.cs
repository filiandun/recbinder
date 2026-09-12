using WindowsInput;
using System.Windows.Forms;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Engine
{
    public class SafeInputSimulator
    {
        private readonly InputSimulator _inputSimulator;

        private readonly HashSet<int> _pressedKeyboardKeys;
        private readonly HashSet<int> _pressedMouseButtons;


        public SafeInputSimulator()
        {
            this._inputSimulator = new InputSimulator();

            this._pressedKeyboardKeys = new HashSet<int>();
            this._pressedMouseButtons = new HashSet<int>();
        }


        public void KeyboardDown(int keyCode)
        {
            this._inputSimulator.Keyboard.KeyDown((VirtualKeyCode)keyCode);

            this._pressedKeyboardKeys.Add(keyCode);
        }

        public void KeyboardUp(int keyCode)
        {
            this._inputSimulator.Keyboard.KeyUp((VirtualKeyCode)keyCode);

            this._pressedKeyboardKeys.Remove(keyCode);
        }


        public void MouseDown(int button)
        {
            switch ((MouseButtons)button)
            {
                case MouseButtons.Left: this._inputSimulator.Mouse.LeftButtonDown(); break;
                case MouseButtons.Right: this._inputSimulator.Mouse.RightButtonDown(); break;

                case MouseButtons.Middle: this._inputSimulator.Mouse.MiddleButtonDown(); break;

                case MouseButtons.XButton1: this._inputSimulator.Mouse.XButtonDown(1); break;
                case MouseButtons.XButton2: this._inputSimulator.Mouse.XButtonDown(2); break;
            }

            this._pressedMouseButtons.Add(button);
        }

        public void MouseUp(int button) 
        {
            switch ((MouseButtons)button)
            {
                case MouseButtons.Left: this._inputSimulator.Mouse.LeftButtonUp(); break;
                case MouseButtons.Right: this._inputSimulator.Mouse.RightButtonUp(); break;

                case MouseButtons.Middle: this._inputSimulator.Mouse.MiddleButtonUp(); break;

                case MouseButtons.XButton1: this._inputSimulator.Mouse.XButtonUp(1); break;
                case MouseButtons.XButton2: this._inputSimulator.Mouse.XButtonUp(2); break;
            }

            this._pressedMouseButtons.Remove(button);
        }


        public void MouseMoveTo(double x, double y) => this._inputSimulator.Mouse.MoveMouseToPositionOnVirtualDesktop(x, y);
        public void MouseMoveBy(int deltaX, int deltaY) => this._inputSimulator.Mouse.MoveMouseBy(deltaX, deltaY);

        public void MouseVerticalScroll(int delta) => this._inputSimulator.Mouse.VerticalScroll(delta);


        public void ReleaseAll()
        {
            foreach (int keyCode in this._pressedKeyboardKeys)
            {
                this._inputSimulator.Keyboard.KeyUp((VirtualKeyCode)keyCode);
            }

            this._pressedKeyboardKeys.Clear();

            foreach (int mouseButton in this._pressedMouseButtons)
            {
                switch ((MouseButtons)mouseButton)
                {
                    case MouseButtons.Left: this._inputSimulator.Mouse.LeftButtonUp(); break;
                    case MouseButtons.Right: this._inputSimulator.Mouse.RightButtonUp(); break;

                    case MouseButtons.Middle: this._inputSimulator.Mouse.MiddleButtonUp(); break;

                    case MouseButtons.XButton1: this._inputSimulator.Mouse.XButtonUp(1); break;
                    case MouseButtons.XButton2: this._inputSimulator.Mouse.XButtonUp(2); break;
                }
            }

            this._pressedMouseButtons.Clear();
        }
    }
}
