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
        private readonly ILogger<MacroEventPlayer> _logger;

        private readonly SafeInputSimulator _inputSimulator;
        private readonly Stopwatch _stopwatch;

        private readonly Macro _macro;
        private readonly MacroPlaybackOptions _options;

        private volatile bool _isPaused;


        public MacroEventPlayer(ILogger<MacroEventPlayer> logger, Macro macro, MacroPlaybackOptions options)
        {
            this._logger = logger;
            this._inputSimulator = new SafeInputSimulator();

            this._stopwatch = new Stopwatch();

            this._macro = macro;
            this._options = options;

            this._isPaused = false;
        }


        public TimeSpan GetElapsedTime() => this._stopwatch.Elapsed;


        public async Task Start(CancellationToken cancellationToken)
        {
            if (this.HasUnsupportedEvents()) throw new NotSupportedException("Macro contains a unsupported events");

            this._stopwatch.Restart();

            Point startCursorPosition = Cursor.Position;

            MacroEvent firstMoveEvent = this._macro.Events.OfType<MacroEvent>().FirstOrDefault(me => me.Kind == MacroEventKind.MouseMove);  // TODO сделать адекватнее, может null допустимым, так как событий мыши может и не быть
            Point firstCursorPosition = new Point((int)firstMoveEvent.MouseMove.X, (int)firstMoveEvent.MouseMove.Y);

            this._logger.LogDebug("Event loop started. Total duration: {Duration}, Total events: {Count}, Initial cursor: {Position}", this._macro.Metadata.Duration, this._macro.Events.Count, startCursorPosition);

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

        private void PlayEvent(MacroEvent e, Point firstCursorPosition, Point startCursorPosition)
        {
            switch (e.Kind)
            {
                case MacroEventKind.KeyboardPress: this.PlayKeyboardPress(e.KeyboardPress); break;
                case MacroEventKind.MousePress: this.PlayMousePress(e.MousePress); break;
                case MacroEventKind.MouseMove: this.PlayMouseMove(e.MouseMove, firstCursorPosition, startCursorPosition); break;
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

        private void PlayMouseMove(MouseMoveEvent e, Point firstCursorPosition, Point startCursorPosition)
        {
            if (!this._options.MouseMoveIgnored)
            {
                int targetX = this._options.RelativeMouseMovementEnabled ? startCursorPosition.X + ((int)e.X - firstCursorPosition.X) : (int)e.X;
                int targetY = this._options.RelativeMouseMovementEnabled ? startCursorPosition.Y + ((int)e.Y - firstCursorPosition.Y) : (int)e.Y;

                int scaledX = (int)(long)targetX * 65535 / this._macro.Metadata.ResolutionWidth;
                int scaledY = (int)(long)targetY * 65535 / this._macro.Metadata.ResolutionHeight;

                this._inputSimulator.MouseMoveTo(scaledX, scaledY);
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


        private bool HasUnsupportedEvents()
        {
            return this._macro.Events.Any(e => e.Kind is not (MacroEventKind.KeyboardPress or MacroEventKind.MousePress or MacroEventKind.MouseMove or MacroEventKind.MouseWheel));
        }
    }
}
