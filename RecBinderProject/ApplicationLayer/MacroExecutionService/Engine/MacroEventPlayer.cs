using WindowsInput;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using RecBinderProject.Domain;
using RecBinderProject.Domain.MacroEvents;
using RecBinderProject.Domain.MacroEvents.Enums;

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


        public async Task Start(CancellationToken cancellationToken)
        {
            this._stopwatch.Restart();

            Point startCursorPosition = Cursor.Position;

            MacroEvent firstMoveEvent = this._macro.Events.OfType<MacroEvent>().FirstOrDefault(me => me.Kind == MacroEventKind.MouseMove);  // TODO сделать адекватнее, может null допустимым, так как событий мыши может и не быть
            Point firstCursorPosition = new Point((int)firstMoveEvent.MouseMove.X, (int)firstMoveEvent.MouseMove.Y);

            try
            {
                foreach (MacroEvent macroEvent in this._macro.Events)
                {
                    await this.PlayDelay(macroEvent.TimeOffset, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    await this.WaitIfPausedTask(cancellationToken);

                    this.PlayEvent(macroEvent, firstCursorPosition, startCursorPosition);
                }

                // if (!this._options.TrimEnd)
                await this.PlayDelay((int)this._macro.Metadata.Duration.TotalMilliseconds, cancellationToken);
            }
            finally
            {
                this.Stop();
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

        private void Stop()
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

        private void PlayEvent(MacroEvent macroEvent, Point firstCursorPosition, Point startCursorPosition)
        {
            switch (macroEvent.Kind) // TODO проверку на null и исключени ниже, для inputEvent.MouseMove и других event
            {
                case MacroEventKind.KeyboardPress: this.PlayKeyboardPress(macroEvent.KeyboardPress); break;
                case MacroEventKind.MousePress: this.PlayMousePress(macroEvent.MousePress); break;
                case MacroEventKind.MouseMove: this.PlayMouseMove(macroEvent.MouseMove, firstCursorPosition, startCursorPosition); break;
                case MacroEventKind.MouseWheel: this.PlayMouseWheel(macroEvent.MouseWheel); break;

                default: throw new ArgumentOutOfRangeException(macroEvent.Kind.GetType().Name, macroEvent.Kind, "error, unknown type macro event");
            }
        }

        private async Task PlayDelay(int timeOffset, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await this.WaitIfPausedTask(cancellationToken);

                long remainingTime = timeOffset - this._stopwatch.ElapsedMilliseconds;

                if (remainingTime <= 0) break;

                if (remainingTime > 15)
                {
                    await Task.Delay(1, cancellationToken);
                }
                else
                {
                    Thread.Yield();
                }
            }
        }

        private void PlayKeyboardPress(KeyboardPressEvent keyboardPressEvent)
        {
            if (!this._options.KeyboardPressIgnored)
            {
                if (keyboardPressEvent.KeyState == MacroKeyState.Down)
                {
                    this._inputSimulator.Keyboard.KeyDown((WindowsInput.Native.VirtualKeyCode)keyboardPressEvent.KeyCode);
                }
                else if (keyboardPressEvent.KeyState == MacroKeyState.Up)
                {
                    this._inputSimulator.Keyboard.KeyUp((WindowsInput.Native.VirtualKeyCode)keyboardPressEvent.KeyCode);
                }
            }
        }

        private void PlayMousePress(MousePressEvent mousePressEvent)
        {
            if (!this._options.MousePressIgnored)
            {
                if (mousePressEvent.KeyState == MacroKeyState.Down)
                {
                    switch (mousePressEvent.MouseButton)
                    {
                        case MouseButtons.Left: this._inputSimulator.Mouse.LeftButtonDown(); break;
                        case MouseButtons.Right: this._inputSimulator.Mouse.RightButtonDown(); break;

                        case MouseButtons.XButton1: this._inputSimulator.Mouse.XButtonDown(1); break;
                        case MouseButtons.XButton2: this._inputSimulator.Mouse.XButtonDown(2); break;
                    }
                }
                else if (mousePressEvent.KeyState == MacroKeyState.Up)
                {
                    switch (mousePressEvent.MouseButton)
                    {
                        case MouseButtons.Left: this._inputSimulator.Mouse.LeftButtonUp(); break;
                        case MouseButtons.Right: this._inputSimulator.Mouse.RightButtonUp(); break;

                        case MouseButtons.XButton1: this._inputSimulator.Mouse.XButtonUp(1); break;
                        case MouseButtons.XButton2: this._inputSimulator.Mouse.XButtonUp(2); break;
                    }
                }
            }
        }

        private void PlayMouseMove(MouseMoveEvent mouseMoveEvent, Point firstCursorPosition, Point startCursorPosition)
        {
            if (!this._options.MouseMoveIgnored)
            {
                int targetX = this._options.RelativeMouseMovementEnabled ? startCursorPosition.X + ((int)mouseMoveEvent.X - firstCursorPosition.X) : (int)mouseMoveEvent.X;
                int targetY = this._options.RelativeMouseMovementEnabled ? startCursorPosition.Y + ((int)mouseMoveEvent.Y - firstCursorPosition.Y) : (int)mouseMoveEvent.Y;

                int scaledX = (int)(long)targetX * 65535 / this._macro.Metadata.ResolutionWidth;
                int scaledY = (int)(long)targetY * 65535 / this._macro.Metadata.ResolutionHeight;

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
