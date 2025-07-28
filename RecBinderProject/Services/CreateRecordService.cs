using System.Diagnostics;
using System.Windows.Forms;
using RecBinderProject.Inputs;

using Gma.System.MouseKeyHook;
using Application = System.Windows.Application;

namespace RecBinderProject.Services
{
    public class CreateRecordService
    {
        public event Action? OnRecordingStopped;

        private CancellationTokenSource _cancellationTokenSource;
        private Stopwatch _stopwatch;

        private IKeyboardMouseEvents _globalHook;

        private RecordingInput _recordingInput;
        public RecordOptions RecordOptions { get; set; }

        public bool IsRecording { get; private set; }


        public CreateRecordService()
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            this._globalHook = Hook.GlobalEvents();

            this._stopwatch = new Stopwatch();

            this.RecordOptions = new RecordOptions();

            this.IsRecording = false;
        }


        public void StartRecord(RecordingMetadata recordingMetadata)
        {
            this._recordingInput = new RecordingInput(recordingMetadata, new InputEvents());

            this._stopwatch.Restart();
            this.Subscribe();

            this.IsRecording = true;

            _ = this.WaitAndStopAfterDurationAsync(this._cancellationTokenSource.Token);
        }

        private async Task WaitAndStopAfterDurationAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(this._recordingInput.RecordingMetadata.Duration, token);
                this.StopRecord();
            }
            catch (TaskCanceledException)
            {
                
            }
        }

        public void StopRecord()
        {
            if (this.IsRecording)
            {
                this.Unsubscribe();
                this._stopwatch.Stop();

                this.IsRecording = false;

                this._recordingInput.RecordingMetadata.Duration = this._stopwatch.Elapsed;
                ((App)Application.Current).RecordingRepository.Save(this._recordingInput);

                this._cancellationTokenSource.Cancel();
            }
        }


        public void Subscribe()
        {
            this._globalHook.KeyDown += GlobalHookKeyDown;
            this._globalHook.KeyUp += GlobalHookKeyUp;

            this._globalHook.MouseDown += GlobalHookMouseDown;
            this._globalHook.MouseUp += GlobalHookMouseUp;

            this._globalHook.MouseMoveExt += GlobalHookMouseMoveExt;
            this._globalHook.MouseWheelExt += GlobalHook_MouseWheelExt;
        }

        public void Unsubscribe()
        {
            this._globalHook.KeyDown -= GlobalHookKeyDown;
            this._globalHook.KeyUp -= GlobalHookKeyUp;

            this._globalHook.MouseDown -= GlobalHookMouseDown;
            this._globalHook.MouseUp -= GlobalHookMouseUp;

            this._globalHook.MouseMoveExt -= GlobalHookMouseMoveExt;
            this._globalHook.MouseWheelExt -= GlobalHook_MouseWheelExt;

            this._globalHook.Dispose();
        }


        private void GlobalHookKeyUp(object? sender, KeyEventArgs e)
        {
            if (!this.RecordOptions.IgnoreKeyboardPress)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new KeyboardPressInput((int)e.KeyCode, KeyState.Up, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }

        private void GlobalHookKeyDown(object? sender, KeyEventArgs e)
        {
            if (!this.RecordOptions.IgnoreKeyboardPress)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new KeyboardPressInput((int)e.KeyCode, KeyState.Down, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }


        private void GlobalHookMouseDown(object? sender, MouseEventArgs e)
        {
            if (!this.RecordOptions.IgnoreMouseMove)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MousePressInput(e.Button, KeyState.Down, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }
        private void GlobalHookMouseUp(object? sender, MouseEventArgs e)
        {
            if (!this.RecordOptions.IgnoreMouseMove)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MousePressInput(e.Button, KeyState.Up, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }


        private void GlobalHookMouseMoveExt(object? sender, MouseEventExtArgs e)
        {
            if (!this.RecordOptions.IgnoreMouseMove)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MouseMoveInput(e.Location.X, e.Location.Y, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }
        private void GlobalHook_MouseWheelExt(object? sender, MouseEventExtArgs e)
        {
            if (!this.RecordOptions.IgnoreMouseWheel)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MouseWheelInput(e.Delta / 120, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }


        public TimeSpan GetElapsedTime()
        {
            return this._stopwatch.Elapsed;
        }
    }
}
