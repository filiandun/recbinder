using System.Windows.Forms;

using Gma.System.MouseKeyHook;

using RecBinderProject.Models;
using RecBinderProject.Models.Inputs;


namespace RecBinderProject.Services
{
    public class CreateRecordService : BaseRecordService
    {
        private IKeyboardMouseEvents? _globalHook;

        private readonly RecordingInput _recordingInput;

        public CreateRecordService(IRecordingRepository recordingRepository, RecordingMetadata recordingMetadata, RecordOptions recordOptions) : base(recordingRepository, recordOptions)
        {
            this._globalHook = null;
            this._recordingInput = new RecordingInput(recordingMetadata);
        }


        public void StartRecord()
        {
            this._stopwatch.Restart();
            this.Subscribe();

            this._isActive = true;

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
            if (this._isActive)
            {
                this.Unsubscribe();
                this._stopwatch.Stop();

                this._isActive = false;

                this._recordingInput.RecordingMetadata.Duration = this._stopwatch.Elapsed;
                this._recordingInput.RecordingMetadata.CreationDateTime = DateTime.Now;
                this._recordingRepository.Save(this._recordingInput);

                this._cancellationTokenSource.Cancel();

                this.NotifyStopped();
            }
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

                this._globalHook.Dispose();
            }
        }


        private void GlobalHookKeyUp(object? sender, KeyEventArgs e)
        {
            if (!this._recordOptions.KeyboardPressEnabled)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new KeyboardPressInput((int)e.KeyCode, KeyState.Up, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }

        private void GlobalHookKeyDown(object? sender, KeyEventArgs e)
        {
            if (!this._recordOptions.KeyboardPressEnabled)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new KeyboardPressInput((int)e.KeyCode, KeyState.Down, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }


        private void GlobalHookMouseDown(object? sender, MouseEventArgs e)
        {
            if (!this._recordOptions.MousePressEnabled)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MousePressInput(e.Button, KeyState.Down, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }
        private void GlobalHookMouseUp(object? sender, MouseEventArgs e)
        {
            if (!this._recordOptions.MousePressEnabled)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MousePressInput(e.Button, KeyState.Up, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }


        private void GlobalHookMouseMoveExt(object? sender, MouseEventExtArgs e)
        {
            if (!this._recordOptions.MouseMoveEnabled)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MouseMoveInput(e.Location.X, e.Location.Y, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }
        private void GlobalHook_MouseWheelExt(object? sender, MouseEventExtArgs e)
        {
            if (!this._recordOptions.MouseWheelEnabled)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MouseWheelInput(e.Delta / 120, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }
    }
}
