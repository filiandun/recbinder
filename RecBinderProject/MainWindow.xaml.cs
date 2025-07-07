using Gma.System.MouseKeyHook;// библиотека для считывания нажатия клавиш и движения мышью
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput; 

namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private IKeyboardMouseEvents m_GlobalHook;

        private RecordedEvents recordedEvents;

        public MainWindow()
        {
            InitializeComponent();

            this.recordedEvents = new RecordedEvents();
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

            m_GlobalHook.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Subscribe();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Unsubscribe();
        }

        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(async () => { await this.recordedEvents.PlayEventsAsync(); }) { IsBackground = true };
            thread.Start();
        }
    }
}