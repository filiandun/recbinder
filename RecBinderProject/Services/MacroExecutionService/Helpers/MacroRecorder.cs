using System.Diagnostics;
using System.Windows.Forms;

using Gma.System.MouseKeyHook;

using RecBinderProject.Models;
using RecBinderProject.Models.InputEvents;


namespace RecBinderProject.Services.MacroExecutionService.Helpers
{
    public class MacroRecorder
    {
        private IKeyboardMouseEvents? _globalHook;

        private readonly Stopwatch _stopwatch;

        private readonly Macro _macro;
        private readonly MacroOptions _recordOptions;

        public MacroRecorder(Macro recordingInput, MacroOptions recordOptions)
        {
            this._globalHook = null;

            this._stopwatch = new Stopwatch();

            this._macro = recordingInput;
            this._recordOptions = recordOptions;
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

            this._globalHook.KeyDown += GlobalHookKeyDown;
            this._globalHook.KeyUp += GlobalHookKeyUp;

            this._globalHook.MouseDown += GlobalHookMouseDown;
            this._globalHook.MouseUp += GlobalHookMouseUp;

            this._globalHook.MouseMoveExt += GlobalHookMouseMoveExt;
            this._globalHook.MouseWheelExt += GlobalHook_MouseWheelExt;
        }

        public void Unsubscribe()
        {
            if (this._globalHook != null)
            {
                this._globalHook.KeyDown -= GlobalHookKeyDown;
                this._globalHook.KeyUp -= GlobalHookKeyUp;

                this._globalHook.MouseDown -= GlobalHookMouseDown;
                this._globalHook.MouseUp -= GlobalHookMouseUp;

                this._globalHook.MouseMoveExt -= GlobalHookMouseMoveExt;
                this._globalHook.MouseWheelExt -= GlobalHook_MouseWheelExt;
            }
        }


        private void GlobalHookKeyUp(object? sender, KeyEventArgs e) // TODO переименовать KeyboardPressEnabled, так как сейчас они логически бредовые получаются (посмотри на if внимательно, если не понял)
        {
            if (!this._recordOptions.KeyboardPressEnabled)
            {
                this._macro.AddEvent(InputEvent.From(new KeyboardPressEvent((int)e.KeyCode, KeyState.Up, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }

        private void GlobalHookKeyDown(object? sender, KeyEventArgs e)
        {
            if (!this._recordOptions.KeyboardPressEnabled)
            {
                this._macro.AddEvent(InputEvent.From(new KeyboardPressEvent((int)e.KeyCode, KeyState.Down, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }


        private void GlobalHookMouseDown(object? sender, MouseEventArgs e)
        {
            if (!this._recordOptions.MousePressEnabled)
            {
                this._macro.AddEvent(InputEvent.From(new MousePressEvent(e.Button, KeyState.Down, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }

        private void GlobalHookMouseUp(object? sender, MouseEventArgs e)
        {
            if (!this._recordOptions.MousePressEnabled)
            {
                this._macro.AddEvent(InputEvent.From(new MousePressEvent(e.Button, KeyState.Up, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }


        private void GlobalHookMouseMoveExt(object? sender, MouseEventExtArgs e)
        {
            if (!this._recordOptions.MouseMoveEnabled)
            {
                this._macro.AddEvent(InputEvent.From(new MouseMoveEvent(e.Location.X, e.Location.Y, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }

        private void GlobalHook_MouseWheelExt(object? sender, MouseEventExtArgs e)
        {
            if (!this._recordOptions.MouseWheelEnabled)
            {
                this._macro.AddEvent(InputEvent.From(new MouseWheelEvent(e.Delta / 120, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }
    }
}