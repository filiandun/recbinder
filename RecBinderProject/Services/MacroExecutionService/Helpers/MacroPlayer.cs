using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using WindowsInput;

using RecBinderProject.Models.InputEvents;
using RecBinderProject.Models;


namespace RecBinderProject.Services.MacroExecutionService.Helpers
{
    public class MacroPlayer
    {
        private CancellationTokenSource _playbackCancellationTokenSource;

        private readonly InputSimulator _inputSimulator;

        private readonly Stopwatch _stopwatch;
        private volatile bool _isPaused;

        private readonly Macro _macro;
        private readonly MacroOptions _macroOptions;

        public MacroPlayer(Macro recordingInput, MacroOptions recordOptions)
        {
            this._playbackCancellationTokenSource = new CancellationTokenSource();

            this._inputSimulator = new InputSimulator();

            this._stopwatch = new Stopwatch();
            this._isPaused = false;

            this._macro = recordingInput;
            this._macroOptions = recordOptions;
        }

        public TimeSpan GetElapsedTime() => this._stopwatch.Elapsed;

        public void Start()
        {
            try
            {
                this._playbackCancellationTokenSource = new CancellationTokenSource();

                this._stopwatch.Restart();

                foreach (InputEvent inputEvent in this._macro.Events)
                {
                    this.PlayDelay(inputEvent.TimeOffset, this._playbackCancellationTokenSource.Token);

                    this._playbackCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    this.WaitIfPaused();

                    this.PlayEvent(inputEvent);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this._stopwatch.Stop();
            }
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
            this._playbackCancellationTokenSource.Cancel();
        }

        private void WaitIfPaused()
        {
            while (this._isPaused)
            {
                this._playbackCancellationTokenSource.Token.ThrowIfCancellationRequested();
                Thread.Sleep(50);
            }
        }

        private void PlayEvent(InputEvent inputEvent)
        {
            switch (inputEvent.Kind) // TODO проверку на null и исключени ниже, для inputEvent.MouseMove и других event
            {
                case InputEventKind.KeyboardPress: this.PlayKeyboardPress((KeyboardPressEvent)inputEvent.KeyboardPress); break;
                case InputEventKind.MousePress: this.PlayMousePress((MousePressEvent)inputEvent.MousePress); break;
                case InputEventKind.MouseMove: this.PlayMouseMove((MouseMoveEvent)inputEvent.MouseMove); break;
                case InputEventKind.MouseWheel: this.PlayMouseWheel((MouseWheelEvent)inputEvent.MouseWheel); break;

                default: throw new Exception("Ошибка. Неизвестное событие, запись повреждена.");
            }
        }

        private void PlayDelay(double timeOffset, CancellationToken cancellationToken) // TODO попытаться оптимизировать, так как очень важная функция при воспроизведении
        {
            long targetTime = this._stopwatch.ElapsedMilliseconds + (long)timeOffset;

            while (this._stopwatch.ElapsedMilliseconds < timeOffset)
            {
                cancellationToken.ThrowIfCancellationRequested();
                this.WaitIfPaused();

                long remaining = targetTime - this._stopwatch.ElapsedMilliseconds;

                if (remaining > 15)
                {
                    Thread.Sleep(1);
                }
                else if (remaining > 0)
                {
                    Thread.SpinWait(100);
                }
            }
        }

        private void PlayKeyboardPress(KeyboardPressEvent keyboardPressInput)
        {
            if (!this._macroOptions.KeyboardPressEnabled)
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
            if (!this._macroOptions.MousePressEnabled)
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

        private void PlayMouseMove(MouseMoveEvent mouseMoveInput)
        {
            if (!this._macroOptions.MouseMoveEnabled)
            {
                Point startPosition = Cursor.Position;

                int targetX = this._macroOptions.RelativeMouseMovementEnabled ? (startPosition.X - (int)mouseMoveInput.X) + (int)mouseMoveInput.X : (int)mouseMoveInput.X;
                int targetY = this._macroOptions.RelativeMouseMovementEnabled ? (startPosition.Y - (int)mouseMoveInput.Y) + (int)mouseMoveInput.Y : (int)mouseMoveInput.Y;

                int scaledX = targetX * 65535 / Screen.PrimaryScreen.Bounds.Width; // TODO подтягивать разрешение монитора не из системы текущее, а из настроек
                int scaledY = targetY * 65535 / Screen.PrimaryScreen.Bounds.Height;

                this._inputSimulator.Mouse.MoveMouseTo(scaledX, scaledY);
            }
        }

        private void PlayMouseWheel(MouseWheelEvent mouseWheelInput)
        {
            if (!this._macroOptions.MouseWheelEnabled)
            {
                this._inputSimulator.Mouse.VerticalScroll(mouseWheelInput.Delta);
            }
        }
    }
}
