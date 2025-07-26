using Gma.System.MouseKeyHook;
using RecBinderProject.Inputs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace RecBinderProject.Services
{
    public class CreateRecordService
    {
        public event EventHandler? OnStopRecording;

        private CancellationTokenSource _cancellationTokenSource;

        private IKeyboardMouseEvents _globalHook;

        private Stopwatch _stopwatch;

        private RecordingInput _recordingInput;

        public CreateRecordService()
        {
            this._cancellationTokenSource = new CancellationTokenSource();
            
            this._stopwatch = new Stopwatch();
        }

        public void StartRecord(RecordingMetadata recordingMetadata)
        {
            this._recordingInput = new RecordingInput(recordingMetadata, new InputEvents());

            this._stopwatch.Restart();
            this.Subscribe();

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
            this.OnStopRecording?.Invoke(this, EventArgs.Empty);
            this._cancellationTokenSource.Cancel();

            this._stopwatch.Stop();

            this.Unsubscribe();
        }


        public void Subscribe()
        {
            this._stopwatch.Restart();

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
            this._recordingInput.RecordingMetadata.Duration = this._stopwatch.Elapsed;

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
            this._recordingInput.InputEvents.Add(InputEvent.From(new KeyboardPressInput((int)e.KeyCode, KeyState.Up, (int)this._stopwatch.ElapsedMilliseconds)));
        }

        private void GlobalHookKeyDown(object? sender, KeyEventArgs e)
        {
            this._recordingInput.InputEvents.Add(InputEvent.From(new KeyboardPressInput((int)e.KeyCode, KeyState.Down, (int)this._stopwatch.ElapsedMilliseconds)));
        }


        private void GlobalHookMouseDown(object? sender, MouseEventArgs e)
        {
            this._recordingInput.InputEvents.Add(InputEvent.From(new MousePressInput(e.Button, KeyState.Down, (int)this._stopwatch.ElapsedMilliseconds)));
        }
        private void GlobalHookMouseUp(object? sender, MouseEventArgs e)
        {
            this._recordingInput.InputEvents.Add(InputEvent.From(new MousePressInput(e.Button, KeyState.Up, (int)this._stopwatch.ElapsedMilliseconds)));
        }


        private void GlobalHookMouseMoveExt(object? sender, MouseEventExtArgs e)
        {
            this._recordingInput.InputEvents.Add(InputEvent.From(new MouseMoveInput(e.Location.X, e.Location.Y, (int)this._stopwatch.ElapsedMilliseconds)));
        }
        private void GlobalHook_MouseWheelExt(object? sender, MouseEventExtArgs e)
        {
            this._recordingInput.InputEvents.Add(InputEvent.From(new MouseWheelInput(e.Delta / 120, (int)this._stopwatch.ElapsedMilliseconds)));
        }


        public RecordingInput GetRecordingInput()
        {
            if (!this._cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException($"Невозможно получить запись, когда та не была ещё остановлена ({this._recordingInput.RecordingMetadata.Name})");
            }

            return this._recordingInput;
        }

        public TimeSpan GetElapsedTime()
        {
            return this._stopwatch.Elapsed;
        }
    }
}
