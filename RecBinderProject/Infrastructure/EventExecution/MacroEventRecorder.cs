using System.Diagnostics;
using System.Windows.Forms;

using Microsoft.Extensions.Logging;

using Gma.System.MouseKeyHook;

using RecBinderProject.ApplicationLayer.MacroExecution.Abstractions;
using RecBinderProject.ApplicationLayer.MacroExecution.Recording;

using RecBinderProject.Domain;
using RecBinderProject.Domain.MacroEvents;
using RecBinderProject.Domain.MacroEvents.Enums;

using RecBinderProject.Infrastructure.Input;


namespace RecBinderProject.Infrastructure.EventExecution
{
    public class MacroEventRecorder : IMacroEventRecorder
    {
        private const int WHEEL_DELTA = 120;

        private readonly ILogger<MacroEventRecorder> _logger;

        private readonly IInputListener _inputListener;
        private IKeyboardMouseEvents? _globalHook;

        private readonly Stopwatch _stopwatch;

        private readonly MacroRecordingOptions _options;
        private readonly Macro _macro;

        public TimeSpan GetElapsedTime => this._stopwatch.Elapsed;


        public MacroEventRecorder(ILogger<MacroEventRecorder> logger, IInputListener inputListener, MacroRecordingOptions recordingOptions, Macro macro)
        {
            this._logger = logger;

            this._inputListener = inputListener;

            this._globalHook = null;

            this._stopwatch = new Stopwatch();

            this._options = recordingOptions;
            this._macro = macro;
        }


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
        }


        private void Subscribe()
        {
            this._globalHook = Hook.GlobalEvents();

            this._globalHook.KeyDown += this.OnKeyDown;
            this._globalHook.KeyUp += this.OnHookKeyUp;

            this._globalHook.MouseDown += this.OnMouseDown;
            this._globalHook.MouseUp += this.OnMouseUp;

            this._globalHook.MouseMoveExt += this.OnMouseMoveExt;
            this._globalHook.MouseWheelExt += this.OnMouseWheelExt;

            this._inputListener.Register();
            this._inputListener.MouseDeltaReceived += this.OnMouseDeltaReceived;
        }


        private void Unsubscribe()
        {
            if (this._globalHook != null)
            {
                this._globalHook.KeyDown -= this.OnKeyDown;
                this._globalHook.KeyUp -= this.OnHookKeyUp;

                this._globalHook.MouseDown -= this.OnMouseDown;
                this._globalHook.MouseUp -= this.OnMouseUp;

                this._globalHook.MouseMoveExt -= this.OnMouseMoveExt;
                this._globalHook.MouseWheelExt -= this.OnMouseWheelExt;

                this._globalHook.Dispose();
                this._globalHook = null;
            }

            this._inputListener.Unregister();
            this._inputListener.MouseDeltaReceived -= this.OnMouseDeltaReceived;
        }


        private void OnHookKeyUp(object? sender, KeyEventArgs e)
        {
            if (!this._options.KeyboardPressIgnored)
            {
                this._logger.LogDebug("Key {State}: {KeyCode}", MacroKeyState.Up, e.KeyCode);

                this._macro.AddEvent(MacroEvent.FromKeyboardPress((int)e.KeyCode, MacroKeyState.Up, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (!this._options.KeyboardPressIgnored)
            {
                this._logger.LogDebug("Key {State}: {KeyCode}", MacroKeyState.Down, e.KeyCode);

                this._macro.AddEvent(MacroEvent.FromKeyboardPress((int)e.KeyCode, MacroKeyState.Down, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }


        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (!this._options.MousePressIgnored)
            {
                this._logger.LogDebug("Mouse {State}: Button {Button}", MacroKeyState.Down, e.Button);

                this._macro.AddEvent(MacroEvent.FromMousePress((int)e.Button, MacroKeyState.Down, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (!this._options.MousePressIgnored)
            {
                this._logger.LogDebug("Mouse {State}: Button {Button}", MacroKeyState.Up, e.Button);

                this._macro.AddEvent(MacroEvent.FromMousePress((int)e.Button, MacroKeyState.Up, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }


        private void OnMouseMoveExt(object? sender, MouseEventExtArgs e)
        {
            if (!this._options.MouseMoveIgnored && !this._options.ThreeDimensionalMouseMovementEnabled)
            {
                this._macro.AddEvent(MacroEvent.FromMouseMove(e.Location.X, e.Location.Y, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }

        private void OnMouseDeltaReceived(MouseDeltaEvent e)
        {
            if (!this._options.MouseMoveIgnored && this._options.ThreeDimensionalMouseMovementEnabled)
            {
                this._macro.AddEvent(MacroEvent.FromMouseDelta(e.DeltaX, e.DeltaY, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }


        private void OnMouseWheelExt(object? sender, MouseEventExtArgs e)
        {
            if (!this._options.MouseWheelIgnored)
            {
                int wheelClicks = (int)Math.Round((double)e.Delta / WHEEL_DELTA);

                this._logger.LogDebug("Mouse wheel: WheelClicks {WheelClicks}", wheelClicks);

                this._macro.AddEvent(MacroEvent.FromMouseWheel(wheelClicks, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }


        public void Dispose()
        {
            this.Unsubscribe();
        }
    }
}