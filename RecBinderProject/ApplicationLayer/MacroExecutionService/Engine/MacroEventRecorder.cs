using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

using Gma.System.MouseKeyHook;

using RecBinderProject.Domain;
using RecBinderProject.Domain.MacroEvents;
using RecBinderProject.Domain.MacroEvents.Enums;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Engine
{
    public class MacroEventRecorder
    {
        private const int WHEEL_DELTA = 120;

        private readonly ILogger<MacroEventRecorder> _logger;

        private IKeyboardMouseEvents? _globalHook;

        private readonly Stopwatch _stopwatch;

        private readonly Macro _macro;
        private readonly MacroRecordingOptions _options;


        public MacroEventRecorder(ILogger<MacroEventRecorder> logger, Macro macro, MacroRecordingOptions options)
        {
            this._logger = logger;

            this._globalHook = null;

            this._stopwatch = new Stopwatch();

            this._macro = macro;
            this._options = options;
        }


        public TimeSpan GetElapsedTime() => this._stopwatch.Elapsed;


        public void Start()
        {
            this._logger.LogDebug("Events hooking started. Duration: {Duration}", this._macro.Metadata.Duration);

            this.Subscribe();
            this._stopwatch.Start();
        }
        public void Resume()
        {
            this._logger.LogDebug("Hooking was resumed at {Elapsed} ms. Events count: {Events}", this._stopwatch.ElapsedMilliseconds, this._macro.Events.Count);

            this.Start();
        }

        public void Pause()
        {
            this._logger.LogDebug("Hooking was paused at {Elapsed} ms. Events count: {Events}", this._stopwatch.ElapsedMilliseconds, this._macro.Events.Count);

            this.Unsubscribe();
            this._stopwatch.Stop();
        }

        public void Stop()
        {
            this._logger.LogDebug("Hooking was stopped at {Elapsed} ms. Events count: {Events}", this._stopwatch.ElapsedMilliseconds, this._macro.Events.Count);

            this.Unsubscribe();
            this._stopwatch.Stop();

            this._globalHook?.Dispose();
        }


        public void Subscribe()
        {
            this._globalHook = Hook.GlobalEvents();

            this._globalHook.KeyDown += this.GlobalHookKeyDown;
            this._globalHook.KeyUp += this.GlobalHookKeyUp;

            this._globalHook.MouseDown += this.GlobalHookMouseDown;
            this._globalHook.MouseUp += this.GlobalHookMouseUp;

            this._globalHook.MouseMoveExt += this.GlobalHookMouseMoveExt;
            this._globalHook.MouseWheelExt += this.GlobalHook_MouseWheelExt;
        }

        public void Unsubscribe()
        {
            if (this._globalHook != null)
            {
                this._globalHook.KeyDown -= this.GlobalHookKeyDown;
                this._globalHook.KeyUp -= this.GlobalHookKeyUp;

                this._globalHook.MouseDown -= this.GlobalHookMouseDown;
                this._globalHook.MouseUp -= this.GlobalHookMouseUp;

                this._globalHook.MouseMoveExt -= this.GlobalHookMouseMoveExt;
                this._globalHook.MouseWheelExt -= this.GlobalHook_MouseWheelExt;
            }
        }


        private void GlobalHookKeyUp(object? sender, KeyEventArgs e)
        {
            if (!this._options.KeyboardPressIgnored)
            {
                this._logger.LogDebug("Key {State}: {KeyCode}", MacroKeyState.Up, e.KeyCode);

                this._macro.AddEvent(MacroEvent.FromKeyboardPress((int)e.KeyCode, MacroKeyState.Up, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }

        private void GlobalHookKeyDown(object? sender, KeyEventArgs e)
        {
            if (!this._options.KeyboardPressIgnored)
            {
                this._logger.LogDebug("Key {State}: {KeyCode}", MacroKeyState.Down, e.KeyCode);

                this._macro.AddEvent(MacroEvent.FromKeyboardPress((int)e.KeyCode, MacroKeyState.Down, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }


        private void GlobalHookMouseDown(object? sender, MouseEventArgs e)
        {
            if (!this._options.MousePressIgnored)
            {
                this._logger.LogDebug("Mouse {State}: Button {Button}", MacroKeyState.Down, e.Button);

                this._macro.AddEvent(MacroEvent.FromMousePress((int)e.Button, MacroKeyState.Down, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }

        private void GlobalHookMouseUp(object? sender, MouseEventArgs e)
        {
            if (!this._options.MousePressIgnored)
            {
                this._logger.LogDebug("Mouse {State}: Button {Button}", MacroKeyState.Up, e.Button);

                this._macro.AddEvent(MacroEvent.FromMousePress((int)e.Button, MacroKeyState.Up, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }


        private void GlobalHookMouseMoveExt(object? sender, MouseEventExtArgs e)
        {
            if (!this._options.MouseMoveIgnored)
            {
                this._macro.AddEvent(MacroEvent.FromMouseMove(e.Location.X, e.Location.Y, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }

        private void GlobalHook_MouseWheelExt(object? sender, MouseEventExtArgs e)
        {
            if (!this._options.MouseWheelIgnored)
            {
                int wheelClicks = (int)Math.Round((double)e.Delta / WHEEL_DELTA);
                this._logger.LogDebug("Mouse wheel: WheelClicks {WheelClicks}", wheelClicks);

                this._macro.AddEvent(MacroEvent.FromMouseWheel(wheelClicks, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }
    }
}