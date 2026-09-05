using Gma.System.MouseKeyHook;
using RecBinderProject.ApplicationLayer.MacroExecutionService.Options;
using RecBinderProject.Domain;
using RecBinderProject.Domain.MacroEvents;
using RecBinderProject.Domain.MacroEvents.Enums;
using System.Diagnostics;
using System.Windows.Forms;


namespace RecBinderProject.ApplicationLayer.MacroExecutionService.Engine
{
    public class MacroEventRecorder
    {
        private IKeyboardMouseEvents? _globalHook;

        private readonly Stopwatch _stopwatch;

        private readonly Macro _macro;
        private readonly MacroRecordingOptions _options;


        public MacroEventRecorder(Macro macro, MacroRecordingOptions options)
        {
            this._globalHook = null;

            this._stopwatch = new Stopwatch();

            this._macro = macro;
            this._options = options;
        }


        public TimeSpan GetElapsedTime() => this._stopwatch.Elapsed;


        public void Start()
        {
            this.Subscribe();
            this._stopwatch.Start();
        }
        public void Resume() => this.Start();

        public void Pause()
        {
            this.Unsubscribe();
            this._stopwatch.Stop();
        }

        public void Stop()
        {
            this.Pause();

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
                this._macro.AddEvent(MacroEvent.FromKeyboardPress((int)e.KeyCode, MacroKeyState.Up, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }

        private void GlobalHookKeyDown(object? sender, KeyEventArgs e)
        {
            if (!this._options.KeyboardPressIgnored)
            {
                this._macro.AddEvent(MacroEvent.FromKeyboardPress((int)e.KeyCode, MacroKeyState.Down, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }


        private void GlobalHookMouseDown(object? sender, MouseEventArgs e)
        {
            if (!this._options.MousePressIgnored)
            {
                this._macro.AddEvent(MacroEvent.FromMousePress(e.Button, MacroKeyState.Down, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }

        private void GlobalHookMouseUp(object? sender, MouseEventArgs e)
        {
            if (!this._options.MousePressIgnored)
            {
                this._macro.AddEvent(MacroEvent.FromMousePress(e.Button, MacroKeyState.Up, (int)this._stopwatch.ElapsedMilliseconds));
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
                this._macro.AddEvent(MacroEvent.FromMouseWheel(e.Delta / 120, (int)this._stopwatch.ElapsedMilliseconds));
            }
        }
    }
}