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

namespace RecBinderProject
{
    public partial class RecordingPage : Page
    {
        private IKeyboardMouseEvents m_GlobalHook;

        private InputEvents recordedEvents;


        public RecordingPage()
        {
            InitializeComponent();

            this.recordedEvents = new InputEvents();
        }

        private void ButtonStartRecord_Click(object sender, RoutedEventArgs e)
        {
            this.Subscribe();
        }

        private void ButtonStopRecord_Click(object sender, RoutedEventArgs e)
        {
            this.Unsubscribe();
        }

        private void ButtonPlayRecord_Click(object sender, RoutedEventArgs e)
        {
            //RecordedEventControl? selectedRecordedEventControl = this.navigationView.SelectedItem as RecordedEventControl; // немного не очень такое решение:
            //                                                                                                               // так как непонятно, если будет ошибка:
            //                                                                                                               // пользователь ничего не выбрал и this.recordedEventListBox.SelectedItem вернул null
            //                                                                                                               // или as RecordedEventControl вернул null, так как в this.recordedEventListBox что-то не то находилось
            //if (selectedRecordedEventControl == null)
            //{
            //    System.Windows.Forms.MessageBox.Show("Вы ничего не выбрали!");
            //    return;
            //}
            //Thread thread = new Thread(async () => { await selectedRecordedEventControl.InputEvents.PlayEventsAsync(); }) { IsBackground = true }; // TO DO: переписать под Task
            //thread.Start();
        }


        public Stopwatch stopWatch;

        public void Subscribe()
        {
            this.stopWatch = Stopwatch.StartNew();

            this.m_GlobalHook = Hook.GlobalEvents();

            this.m_GlobalHook.KeyDown += GlobalHookKeyDown;
            this.m_GlobalHook.KeyUp += GlobalHookKeyUp;

            this.m_GlobalHook.MouseDown += GlobalHookMouseDown;
            this.m_GlobalHook.MouseUp += GlobalHookMouseUp;

            this.m_GlobalHook.MouseMoveExt += GlobalHookMouseMoveExt;
        }

        private void GlobalHookKeyUp(object? sender, KeyEventArgs e)
        {
            int timeOffset = (int)this.stopWatch.ElapsedMilliseconds;
            KeyboardPressEvent keyboardPressEvent = new KeyboardPressEvent((int)e.KeyCode, KeyState.Up, timeOffset);

            //this.textBox.Text += "[KU: " + e.KeyValue.ToString() + ":" + timeOffset + " ] ";

            this.recordedEvents.AddEvents(keyboardPressEvent);
        }

        private void GlobalHookKeyDown(object? sender, KeyEventArgs e)
        {
            int timeOffset = (int)this.stopWatch.ElapsedMilliseconds;
            KeyboardPressEvent keyboardPressEvent = new KeyboardPressEvent((int)e.KeyCode, KeyState.Down, timeOffset);

            //this.textBox.Text += "[KD: " + e.KeyValue.ToString() + ":" + timeOffset + " ] ";

            this.recordedEvents.AddEvents(keyboardPressEvent);
        }


        private void GlobalHookMouseDown(object? sender, MouseEventArgs e)
        {
            int timeOffset = (int)this.stopWatch.ElapsedMilliseconds;
            MousePressEvent mousePressEvent = new MousePressEvent(e.Button, KeyState.Down, timeOffset);

            //this.textBox.Text += "[D: " + e.Button.ToString() + ":" + timeOffset + " ] ";

            this.recordedEvents.AddEvents(mousePressEvent);
        }

        private void GlobalHookMouseUp(object? sender, MouseEventArgs e)
        {
            int timeOffset = (int)this.stopWatch.ElapsedMilliseconds;
            MousePressEvent mousePressEvent = new MousePressEvent(e.Button, KeyState.Up, timeOffset);

            //this.textBox.Text += "[U: " + e.Button.ToString() + ":" + timeOffset + " ] ";

            this.recordedEvents.AddEvents(mousePressEvent);
        }


        private void GlobalHookMouseMoveExt(object? sender, MouseEventExtArgs e)
        {
            //this.textBox.Text = e.Location.X + " " + e.Location.Y;
            int timeOffset = (int)this.stopWatch.ElapsedMilliseconds;
            //if (timeOffset - this.previousTimeMouseMove < 10) { return; }

            //this.textBox.Text += "[M: " + e.Location.X.ToString() + "-" + e.Location.Y.ToString() + ":" + timeOffset + " ] ";

            this.recordedEvents.AddEvents(new MouseMoveEvent(e.Location.X, e.Location.Y, (int)this.stopWatch.ElapsedMilliseconds));
        }

        public void Unsubscribe()
        {
            this.m_GlobalHook.KeyDown -= GlobalHookKeyDown;
            this.m_GlobalHook.KeyUp -= GlobalHookKeyUp;

            this.m_GlobalHook.MouseDown -= GlobalHookMouseDown;
            this.m_GlobalHook.MouseUp -= GlobalHookMouseUp;

            this.m_GlobalHook.MouseMoveExt -= GlobalHookMouseMoveExt;

            this.m_GlobalHook.Dispose();

            //this.navigationView.MenuItems.Add(new RecordedEventControl((InputEvents)this.inputEventsList.Clone()));
            this.recordedEvents.ClearEvents();
        }
    }
}
