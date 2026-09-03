using WindowsInput;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using RecBinderProject.Domain;
using RecBinderProject.Domain.MacroEvents;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Engine
{
    public class MacroEventPlayer
    {
        private readonly InputSimulator _inputSimulator;

        private readonly Stopwatch _stopwatch;

        private readonly Macro _macro;
        private readonly MacroPlaybackOptions _options;

        private volatile bool _isPaused;


        public MacroEventPlayer(Macro macro, MacroPlaybackOptions options)
        {
            this._inputSimulator = new InputSimulator();

            this._stopwatch = new Stopwatch();

            this._macro = macro;
            this._options = options;

            this._isPaused = false;
        }


        public TimeSpan GetElapsedTime() => this._stopwatch.Elapsed;

        // TODO баг небольшой, что если запись длится условно 5 секунд, а последнее событие было на 3-ей секунде,
        // то тогда воспроизведение остановится на 3-ей секунде (следовательно таймер тоже, поэтому на странице будет "00:00:03:00 / 00:00:05:00 - Воспроизведение завершено")
        public async Task Start(CancellationToken cancellationToken)
        {
            this._stopwatch.Restart();

            Point startCursorPosition = Cursor.Position;

            foreach (MacroEvent inputEvent in this._macro.Events)
            {
                await this.PlayDelay(inputEvent.TimeOffset, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                await this.WaitIfPausedTask(cancellationToken);

                this.PlayEvent(inputEvent, startCursorPosition);
            }

            this._stopwatch.Stop();
        }

        public void Resume()
        {
            this._isPaused = false;
            this._stopwatch.Start();
        }

        public void Pause()
        {
            this._isPaused = true;
            this._stopwatch.Stop();
        }

        public void Stop()
        {
            this._isPaused = false;
            this._stopwatch.Stop();
        }


        private async Task WaitIfPausedTask(CancellationToken cancellationToken)
        {
            while (this._isPaused)
            {
                await Task.Delay(50, cancellationToken);
            }
        }

        private void PlayEvent(MacroEvent inputEvent, Point cursorPosition)
        {
            switch (inputEvent.Kind) // TODO проверку на null и исключени ниже, для inputEvent.MouseMove и других event
            {
                case InputEventKind.KeyboardPress: this.PlayKeyboardPress((KeyboardPressEvent)inputEvent.KeyboardPress); break;
                case InputEventKind.MousePress: this.PlayMousePress((MousePressEvent)inputEvent.MousePress); break;
                case InputEventKind.MouseMove: this.PlayMouseMove((MouseMoveEvent)inputEvent.MouseMove, cursorPosition); break;
                case InputEventKind.MouseWheel: this.PlayMouseWheel((MouseWheelEvent)inputEvent.MouseWheel); break;

                default: throw new ArgumentOutOfRangeException(inputEvent.Kind.GetType().Name, inputEvent.Kind, "error, unknown type input event");
            }
        }

        private async Task PlayDelay(double timeOffset, CancellationToken cancellationToken) // TODO попытаться оптимизировать, так как очень важная функция при воспроизведении
        {
            long targetTime = (long)timeOffset;

            while (this._stopwatch.ElapsedMilliseconds < targetTime)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await this.WaitIfPausedTask(cancellationToken);
                await Task.Delay(1, cancellationToken);
            }
        }

        private void PlayKeyboardPress(KeyboardPressEvent keyboardPressInput)
        {
            if (!this._options.KeyboardPressIgnored)
            {
                if (keyboardPressInput.KeyState == KeyState.Down)
                {
                    this._inputSimulator.Keyboard.KeyDown((WindowsInput.Native.VirtualKeyCode)keyboardPressInput.KeyCode);
                }
                else if (keyboardPressInput.KeyState == KeyState.Up)
                {
                    this._inputSimulator.Keyboard.KeyUp((WindowsInput.Native.VirtualKeyCode)keyboardPressInput.KeyCode);
                }
            }
        }

        private void PlayMousePress(MousePressEvent mousePressInput)
        {
            if (!this._options.MousePressIgnored)
            {
                if (mousePressInput.KeyState == KeyState.Down)
                {
                    switch (mousePressInput.MouseButton)
                    {
                        case MouseButtons.Left: this._inputSimulator.Mouse.LeftButtonDown(); break;
                        case MouseButtons.Right: this._inputSimulator.Mouse.RightButtonDown(); break;

                        case MouseButtons.XButton1: this._inputSimulator.Mouse.XButtonDown(1); break;
                        case MouseButtons.XButton2: this._inputSimulator.Mouse.XButtonDown(2); break;
                    }
                }
                else if (mousePressInput.KeyState == KeyState.Up)
                {
                    switch (mousePressInput.MouseButton)
                    {
                        case MouseButtons.Left: this._inputSimulator.Mouse.LeftButtonUp(); break;
                        case MouseButtons.Right: this._inputSimulator.Mouse.RightButtonUp(); break;

                        case MouseButtons.XButton1: this._inputSimulator.Mouse.XButtonUp(1); break;
                        case MouseButtons.XButton2: this._inputSimulator.Mouse.XButtonUp(2); break;
                    }
                }
            }
        }

        private void PlayMouseMove(MouseMoveEvent mouseMoveInput, Point cursorPosition)
        {
            if (!this._options.MouseMoveIgnored)
            {
                MacroEvent firstMoveEvent = this._macro.Events.OfType<MacroEvent>().FirstOrDefault(ie => ie.MouseMove is not null);
                Point firstPosition = new Point((int)firstMoveEvent.MouseMove.Value.X, (int)firstMoveEvent.MouseMove.Value.Y);

                int targetX = this._options.RelativeMouseMovementEnabled ? cursorPosition.X + ((int)mouseMoveInput.X - firstPosition.X) : (int)mouseMoveInput.X;
                int targetY = this._options.RelativeMouseMovementEnabled ? cursorPosition.Y + ((int)mouseMoveInput.Y - firstPosition.Y) : (int)mouseMoveInput.Y;

                int scaledX = targetX * 65535 / Screen.PrimaryScreen.Bounds.Width; // TODO подтягивать разрешение монитора надо из настроек
                int scaledY = targetY * 65535 / Screen.PrimaryScreen.Bounds.Height;

                this._inputSimulator.Mouse.MoveMouseTo(scaledX, scaledY);
            }
        }

        private void PlayMouseWheel(MouseWheelEvent mouseWheelInput)
        {
            if (!this._options.MouseWheelIgnored)
            {
                this._inputSimulator.Mouse.VerticalScroll(mouseWheelInput.Delta);
            }
        }
    }
}
