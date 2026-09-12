using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;

using RecBinderProject.Domain;
using RecBinderProject.Domain.MacroEvents;
using RecBinderProject.Domain.MacroEvents.Enums;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Engine
{
    public class MacroEventPlayer
    {
        private const int VIRTUAL_SCREEN_MAX = 65535;

        private readonly ILogger<MacroEventPlayer> _logger;

        private readonly SafeInputSimulator _inputSimulator;
        private readonly Stopwatch _stopwatch;

        private readonly Macro _macro;
        private readonly MacroPlaybackOptions _options;

        private volatile bool _isPaused;

        private Point _mouseOffset;


        public MacroEventPlayer(ILogger<MacroEventPlayer> logger, Macro macro, MacroPlaybackOptions options)
        {
            this._logger = logger;
            this._inputSimulator = new SafeInputSimulator();

            this._stopwatch = new Stopwatch();

            this._macro = macro;
            this._options = options;

            this._isPaused = false;

            this._mouseOffset = Point.Empty;
        }


        public TimeSpan GetElapsedTime() => this._stopwatch.Elapsed;


        public async Task Start(CancellationToken cancellationToken)
        {
            if (this.HasUnsupportedEvents()) throw new NotSupportedException("Macro contains a unsupported kind events");

            this._stopwatch.Restart();

            this.CalcMouseOffset();

            this._logger.LogDebug("Event loop started. Total duration: {Duration}, Total events: {Count}, Mouse offset: {MouseOffset}", this._macro.Metadata.Duration, this._macro.Events.Count, this._mouseOffset);

            try
            {
                foreach (MacroEvent macroEvent in this._macro.Events)
                {
                    await this.PlayDelay(macroEvent.TimeOffset, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    await this.WaitIfPausedTask(cancellationToken);

                    this.PlayEvent(macroEvent);
                }

                // if (!this._options.TrimEnd)
                await this.PlayDelay((int)this._macro.Metadata.Duration.TotalMilliseconds, cancellationToken);
            }
            finally
            {
                this._logger.LogDebug("Event loop finally. Elapsed time: {Elapsed} ms", this._stopwatch.ElapsedMilliseconds);

                this.Stop();
                this._inputSimulator.ReleaseAll();
            }
        }

        public void Resume()
        {
            this._logger.LogDebug("Stopwatch was resumed at {Elapsed} ms", this._stopwatch.ElapsedMilliseconds);

            this._isPaused = false;
            this._stopwatch.Start();
        }

        public void Pause()
        {
            this._logger.LogDebug("Stopwatch was paused at {Elapsed} ms", this._stopwatch.ElapsedMilliseconds);

            this._isPaused = true;
            this._stopwatch.Stop();
        }

        private void Stop()
        {
            this._logger.LogDebug("Stopwatch was stopped at {Elapsed} ms", this._stopwatch.ElapsedMilliseconds);

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

        private void PlayEvent(MacroEvent e)
        {
            switch (e.Kind)
            {
                case MacroEventKind.KeyboardPress: this.PlayKeyboardPress(e.KeyboardPress); break;
                case MacroEventKind.MousePress: this.PlayMousePress(e.MousePress); break;

                case MacroEventKind.MouseMove: this.PlayMouseMove(e.MouseMove); break;
                case MacroEventKind.MouseDelta: this.PlayMouseDelta(e.MouseDelta); break;

                case MacroEventKind.MouseWheel: this.PlayMouseWheel(e.MouseWheel); break;

                default: throw new ArgumentOutOfRangeException(nameof(e.Kind), e.Kind, "Unknown macro event kind");
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

        private void PlayKeyboardPress(KeyboardPressEvent e)
        {
            if (!this._options.KeyboardPressIgnored)
            {
                this._logger.LogDebug("Key {State}: {KeyCode}", e.KeyState, e.KeyCode);

                switch (e.KeyState)
                {
                    case MacroKeyState.Down: this._inputSimulator.KeyboardDown(e.KeyCode); break;
                    case MacroKeyState.Up: this._inputSimulator.KeyboardUp(e.KeyCode); break;

                    default: throw new ArgumentOutOfRangeException(nameof(e.KeyState), e.KeyState, "Unknown keyboard key state");
                }
            }
        }

        private void PlayMousePress(MousePressEvent e)
        {
            if (!this._options.MousePressIgnored)
            {
                this._logger.LogDebug("Mouse {State}: Button {Button}", e.KeyState, e.MouseButton);

                switch (e.KeyState)
                {
                    case MacroKeyState.Down: this._inputSimulator.MouseDown(e.MouseButton); break;
                    case MacroKeyState.Up: this._inputSimulator.MouseUp(e.MouseButton); break;

                    default: throw new ArgumentOutOfRangeException(nameof(e.KeyState), e.KeyState, "Unknown mouse button state");
                }
            }
        }

        private void PlayMouseMove(MouseMoveEvent e)
        {
            if (!this._options.MouseMoveIgnored)
            {
                double targetX = e.X + this._mouseOffset.X;
                double targetY = e.Y + this._mouseOffset.Y;

                double virtualX = targetX * VIRTUAL_SCREEN_MAX / (this._macro.Metadata.ResolutionWidth - 1);
                double virtualY = targetY * VIRTUAL_SCREEN_MAX / (this._macro.Metadata.ResolutionHeight - 1);

                this._inputSimulator.MouseMoveTo(virtualX, virtualY);
            }
        }


        private void PlayMouseDelta(MouseDeltaEvent e)
        {
            if (!this._options.MouseMoveIgnored)
            {
                this._inputSimulator.MouseMoveBy(e.DeltaX, e.DeltaY);
            }
        }

        private void PlayMouseWheel(MouseWheelEvent e)
        {
            if (!this._options.MouseWheelIgnored)
            {
                this._logger.LogDebug("Mouse wheel: WheelClicks {WheelClicks}", e.WheelClicks);

                this._inputSimulator.MouseVerticalScroll(e.WheelClicks);
            }
        }


        private void CalcMouseOffset()
        {
            if (this._options.RelativeMouseMovementEnabled)
            {
                int moveIndex = this._macro.Events.FindIndex(me => me.Kind == MacroEventKind.MouseMove);

                if (moveIndex != -1)
                {
                    Point startCursorPosition = Cursor.Position;

                    MacroEvent firstMove = this._macro.Events[moveIndex];
                    Point firstCursorPosition = new Point((int)firstMove.MouseMove.X, (int)firstMove.MouseMove.Y);

                    this._mouseOffset = new Point(startCursorPosition.X - firstCursorPosition.X, startCursorPosition.Y - firstCursorPosition.Y);
                }
            }
            else
            {
                this._mouseOffset = Point.Empty;
            }
        }

        private bool HasUnsupportedEvents()
        {
            return this._macro.Events.Any(e => e.Kind is not (MacroEventKind.KeyboardPress or MacroEventKind.MousePress or MacroEventKind.MouseMove or MacroEventKind.MouseDelta or MacroEventKind.MouseWheel));
        }
    }
}
