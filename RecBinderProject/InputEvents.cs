using WindowsInput;
using System.Diagnostics;
using System.Windows.Forms;

using RecBinderProject.Inputs;

namespace RecBinderProject
{
    public class InputEvents
    {
        public LinkedList<InputEvent> InputEventsList { get; init; }

        public InputEvents()
        {
            this.InputEventsList = new LinkedList<InputEvent>();
        }

        public void Add(InputEvent userEvent)
        {
            this.InputEventsList.AddLast(userEvent);
        }

        public void Clear()
        {
            this.InputEventsList.Clear();
        }


        public async Task PlayAsync(RecordOptions recordOptions, CancellationToken cancellationToken)
        {
            InputSimulator inputSimulator = new InputSimulator();
            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (InputEvent inputEvent in this.InputEventsList)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await this.PlayDelayAsync(stopwatch, inputEvent.TimeOffset, stopwatch.ElapsedMilliseconds);

                switch (inputEvent.Kind) // TO DO проверку на null и исключени ниже, для inputEvent.MouseMove и других event
                {
                    case InputEventKind.KeyboardPress: if (!recordOptions.IgnoreKeyboardPress) { this.PlayKeyboardPress((KeyboardPressInput)inputEvent.KeyboardPress, inputSimulator); }; break;
                    case InputEventKind.MousePress: if (!recordOptions.IgnoreMousePress) { this.PlayMousePress((MousePressInput)inputEvent.MousePress, inputSimulator); }; break;
                    case InputEventKind.MouseMove: if (!recordOptions.IgnoreMouseMove) { this.PlayMouseMove((MouseMoveInput)inputEvent.MouseMove, inputSimulator); }; break;
                    case InputEventKind.MouseWheel: if (!recordOptions.IgnoreMouseWheel) { this.PlayMouseWheel((MouseWheelInput)inputEvent.MouseWheel, inputSimulator); }; break;

                    default: MessageBox.Show("Ошибка. Запись повреждена."); return;
                }  
            }
        }


        private async Task PlayDelayAsync(Stopwatch stopwatch, double timeOffset, double elapsed) // TO DO проверить на работоспособность (GPT предложил)
        {
            //double delay = timeOffset - elapsed;

            //if (delay > 0)
            //{
            //    await Task.Delay((int)delay);
            //}

            while (stopwatch.ElapsedMilliseconds < timeOffset)
            {
                var delay = timeOffset - stopwatch.ElapsedMilliseconds;

                if (delay > 2)
                {
                    await Task.Delay((int)delay / 2);
                }
                else
                {
                    await Task.Yield();
                }
            }
        }

        private void PlayKeyboardPress(KeyboardPressInput keyboardPressInput, InputSimulator inputSimulator)
        {
            if (keyboardPressInput.KeyState == KeyState.Down)
            {
                inputSimulator.Keyboard.KeyDown((WindowsInput.Native.VirtualKeyCode)keyboardPressInput.KeyCode);
            }
            else if (keyboardPressInput.KeyState == KeyState.Up)
            {
                inputSimulator.Keyboard.KeyUp((WindowsInput.Native.VirtualKeyCode)keyboardPressInput.KeyCode);
            }
        }

        private void PlayMousePress(MousePressInput mousePressInput, InputSimulator inputSimulator)
        {
            if (mousePressInput.KeyState == KeyState.Down)
            {
                switch (mousePressInput.MouseButton)
                {
                    case MouseButtons.Left: inputSimulator.Mouse.LeftButtonDown(); break;
                    case MouseButtons.Right: inputSimulator.Mouse.RightButtonDown(); break;

                    case MouseButtons.XButton1: inputSimulator.Mouse.XButtonDown(1); break;
                    case MouseButtons.XButton2: inputSimulator.Mouse.XButtonDown(2); break;
                }
            }
            else if (mousePressInput.KeyState == KeyState.Up)
            {
                switch (mousePressInput.MouseButton)
                {
                    case MouseButtons.Left: inputSimulator.Mouse.LeftButtonUp(); break;
                    case MouseButtons.Right: inputSimulator.Mouse.RightButtonUp(); break;

                    case MouseButtons.XButton1: inputSimulator.Mouse.XButtonUp(1); break;
                    case MouseButtons.XButton2: inputSimulator.Mouse.XButtonUp(2); break;
                }
            }
        }

        private void PlayMouseMove(MouseMoveInput mouseMoveInput, InputSimulator inputSimulator)
        {
            var startPosition = Cursor.Position;

            int targetX = /*startPosition.X +*/ (int)mouseMoveInput.X;
            int targetY = /*startPosition.Y +*/ (int)mouseMoveInput.Y;

            int scaledX = targetX * 65535 / Screen.PrimaryScreen.Bounds.Width;
            int scaledY = targetY * 65535 / Screen.PrimaryScreen.Bounds.Height;

            inputSimulator.Mouse.MoveMouseTo(scaledX, scaledY);
        }

        private void PlayMouseWheel(MouseWheelInput mouseWheelInput, InputSimulator inputSimulator)
        {
            inputSimulator.Mouse.VerticalScroll(mouseWheelInput.Delta);
        }

    }
}
