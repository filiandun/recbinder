using System.Diagnostics;
using System.Windows.Forms;

using Gma.System.MouseKeyHook;

using RecBinderProject.Models;
using RecBinderProject.Models.Inputs;


namespace RecBinderProject.Services
{
    public class CreateRecordService
    {
        public event Action? OnRecordingStopped;

        private IKeyboardMouseEvents? _globalHook;

        private CancellationTokenSource _cancellationTokenSource;
        private Stopwatch _stopwatch;

        private IRecordingRepository _recordingRepository;

        private RecordOptions _recordOptions;

        private RecordingMetadata _recordingMetadata;
        private RecordingInput _recordingInput;

        private bool _isRecording;


        public RecordingMetadata RecordingMetadata 
        { 
            get => this._recordingMetadata;
            set => this._recordingMetadata = value;
        }

        public RecordOptions RecordOptions
        {
            get => this._recordOptions;
            set => this._recordOptions = value;
        }


        public CreateRecordService(IRecordingRepository recordingRepository, RecordOptions recordOptions)
        {
            this._globalHook = null;

            this._cancellationTokenSource = new CancellationTokenSource();
            this._stopwatch = new Stopwatch();

            this._recordingRepository = recordingRepository;

            this._recordOptions = recordOptions;

            this._isRecording = false;
        }


        public void StartRecord()
        {
            if (this.RecordingMetadata == null)
            {
                throw new Exception("Error. Record metadata is null.");
            }

            this._recordingInput = new RecordingInput(this._recordingMetadata, new InputEvents());

            this._stopwatch.Restart();
            this.Subscribe();

            this._isRecording = true;

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
            if (this._isRecording)
            {
                this.Unsubscribe();
                this._stopwatch.Stop();

                this._isRecording = false;

                this._recordingInput.RecordingMetadata.Duration = this._stopwatch.Elapsed;
                this._recordingRepository.Save(this._recordingInput);

                this._cancellationTokenSource.Cancel();

                this.OnRecordingStopped?.Invoke();
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
            if (!this._recordOptions.IgnoreKeyboardPress)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new KeyboardPressInput((int)e.KeyCode, KeyState.Up, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }

        private void GlobalHookKeyDown(object? sender, KeyEventArgs e)
        {
            if (!this._recordOptions.IgnoreKeyboardPress)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new KeyboardPressInput((int)e.KeyCode, KeyState.Down, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }


        private void GlobalHookMouseDown(object? sender, MouseEventArgs e)
        {
            if (!this._recordOptions.IgnoreMouseMove)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MousePressInput(e.Button, KeyState.Down, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }
        private void GlobalHookMouseUp(object? sender, MouseEventArgs e)
        {
            if (!this._recordOptions.IgnoreMouseMove)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MousePressInput(e.Button, KeyState.Up, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }


        private void GlobalHookMouseMoveExt(object? sender, MouseEventExtArgs e)
        {
            if (!this._recordOptions.IgnoreMouseMove)
            {
                this._recordingInput.InputEvents.Add(InputEvent.From(new MouseMoveInput(e.Location.X, e.Location.Y, (int)this._stopwatch.ElapsedMilliseconds)));
            }
        }
        private void GlobalHook_MouseWheelExt(object? sender, MouseEventExtArgs e)
        {
            if (!this._recordOptions.IgnoreMouseWheel)
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
