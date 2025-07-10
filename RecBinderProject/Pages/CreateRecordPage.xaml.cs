using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;

namespace RecBinderProject
{
    public partial class CreateRecordPage : Page
    {
        public event EventHandler StopRecording;

        private IKeyboardMouseEvents m_GlobalHook;
        public RecordingInput RecordingInput { get; private set; }  // TO DO:
        private InputEvents inputEvents;                            // переписать,
        private RecordingMetadata recordingMetadata;                // а то мне не нравится, что и в RecordingInput есть объекты этих классов, а я их тут повторно создаю

        private DispatcherTimer timer;
        public Stopwatch stopWatch;

        public CreateRecordPage()
        {
            InitializeComponent();

            this.timer = new DispatcherTimer();
            this.stopWatch = new Stopwatch();

            this.timer.Interval = TimeSpan.FromMilliseconds(10);
            this.timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeSpan elapsed = stopWatch.Elapsed;
            
            this.buttonStartRecord.Content = elapsed.ToString(@"hh\:mm\:ss\.ff");
        }

        private void ButtonStartRecord_Click(object sender, RoutedEventArgs e)
        {
            this.recordNameTextBox.IsEnabled = false;
            this.recordDescriptionTextBox.IsEnabled = false;
            this.durationGrid.IsEnabled = false;
            this.settingsGrid.IsEnabled = false;

            this.buttonStartRecord.Content = "00:00:00:00";
            this.buttonStartRecord.IsEnabled = false;
            this.buttonStopRecord.IsEnabled = true;

            this.inputEvents = new InputEvents();

            this.Subscribe();
        }

        private void ButtonStopRecord_Click(object sender, RoutedEventArgs e)
        {
            string recordName = this.recordNameTextBox.Text;
            string recordDesciption = this.recordDescriptionTextBox.Text;
            TimeSpan recordDuration = this.stopWatch.Elapsed;
            DateTime recordCreationDateTime = DateTime.Now;

            this.recordingMetadata = new RecordingMetadata(recordName, recordDesciption, recordDuration, recordCreationDateTime);
            this.RecordingInput = new RecordingInput((InputEvents)this.inputEvents.Clone(), this.recordingMetadata);

            this.StopRecording?.Invoke(this, e);
            this.Unsubscribe();

            this.recordNameTextBox.IsEnabled = true;
            this.recordDescriptionTextBox.IsEnabled = true;
            this.durationGrid.IsEnabled = true;
            this.settingsGrid.IsEnabled = true;

            this.buttonStartRecord.Content = "Начать запись";
            this.buttonStartRecord.IsEnabled = true;
            this.buttonStopRecord.IsEnabled = false;

        }

        public void Subscribe()
        {
            this.stopWatch.Restart(); 
            this.timer.Start();

            this.m_GlobalHook = Hook.GlobalEvents();

            this.m_GlobalHook.KeyDown += GlobalHookKeyDown;
            this.m_GlobalHook.KeyUp += GlobalHookKeyUp;

            this.m_GlobalHook.MouseDown += GlobalHookMouseDown;
            this.m_GlobalHook.MouseUp += GlobalHookMouseUp;

            this.m_GlobalHook.MouseMoveExt += GlobalHookMouseMoveExt;
        }

        private void GlobalHookKeyUp(object? sender, KeyEventArgs e)
        {
            this.inputEvents.AddEvent(new KeyboardPressInput((int)e.KeyCode, KeyState.Up, (int)this.stopWatch.ElapsedMilliseconds));
        }

        private void GlobalHookKeyDown(object? sender, KeyEventArgs e)
        {
            this.inputEvents.AddEvent(new KeyboardPressInput((int)e.KeyCode, KeyState.Down, (int)this.stopWatch.ElapsedMilliseconds));
        }


        private void GlobalHookMouseDown(object? sender, MouseEventArgs e)
        {
            this.inputEvents.AddEvent(new MousePressInput(e.Button, KeyState.Down, (int)this.stopWatch.ElapsedMilliseconds));
        }

        private void GlobalHookMouseUp(object? sender, MouseEventArgs e)
        {
            this.inputEvents.AddEvent(new MousePressInput(e.Button, KeyState.Up, (int)this.stopWatch.ElapsedMilliseconds));
        }


        private void GlobalHookMouseMoveExt(object? sender, MouseEventExtArgs e)
        {
            this.inputEvents.AddEvent(new MouseMoveInput(e.Location.X, e.Location.Y, (int)this.stopWatch.ElapsedMilliseconds));
        }

        public void Unsubscribe()
        {
            this.timer.Stop();
            this.stopWatch.Stop();

            this.RecordingInput.RecordingMetadata.Duration = this.stopWatch.Elapsed;

            this.m_GlobalHook.KeyDown -= GlobalHookKeyDown;
            this.m_GlobalHook.KeyUp -= GlobalHookKeyUp;

            this.m_GlobalHook.MouseDown -= GlobalHookMouseDown;
            this.m_GlobalHook.MouseUp -= GlobalHookMouseUp;

            this.m_GlobalHook.MouseMoveExt -= GlobalHookMouseMoveExt;

            this.m_GlobalHook.Dispose();
            
            this.inputEvents.ClearEvents();
        }
    }
}
