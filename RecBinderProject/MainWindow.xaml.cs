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

        private MouseMovements mouseMovements;
        private MouseClicks mouseClicks;

        public MainWindow()
        {
            InitializeComponent();

            this.mouseMovements = new MouseMovements();
            this.mouseClicks = new MouseClicks();
        }

        public int previousTimeMouseClick = 0;
        public int previousTimeMouseMove = 0;

        public Stopwatch stopWatch;

        public void Subscribe()
        {
            this.stopWatch = Stopwatch.StartNew();

            m_GlobalHook = Hook.GlobalEvents();
             
            //m_GlobalHook.KeyPress += GlobalHookKeyPress;

            m_GlobalHook.MouseDown += GlobalHookMouseDown;
            m_GlobalHook.MouseUp += GlobalHookMouseUp;

            m_GlobalHook.MouseMoveExt += GlobalHookMouseMoveExt;
        }


        private void GlobalHookMouseDown(object sender, MouseEventArgs e)
        {
            this.textBox.Text += "Down: " + e.Button.ToString() + "//";
            this.mouseClicks.AddMouseClick(new MouseClick(e.Button, KeyState.Down, this.GetDelay(ref this.previousTimeMouseClick)));
        }

        private void GlobalHookMouseUp(object sender, MouseEventArgs e)
        {
            this.textBox.Text += "Up: " + e.Button.ToString() + "//";
            this.mouseClicks.AddMouseClick(new MouseClick(e.Button, KeyState.Up, this.GetDelay(ref this.previousTimeMouseClick)));
        }



        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            this.textBox.Text += e.KeyChar.ToString() + " ";
        }

        private void GlobalHookMouseMoveExt(object sender, MouseEventExtArgs e)
        {
            //this.textBox.Text = e.Location.X + " " + e.Location.Y;
            int currentTime = (int)this.stopWatch.ElapsedMilliseconds;
            if (currentTime - this.previousTimeMouseMove < 10) { return; }
            this.mouseMovements.AddMouseMovement(new MouseMove(e.Location.X, e.Location.Y, this.GetDelay(ref this.previousTimeMouseMove)));
        }

        public void Unsubscribe()
        {
            m_GlobalHook.MouseDown -= GlobalHookMouseDown;
            m_GlobalHook.MouseUp -= GlobalHookMouseUp;

            m_GlobalHook.KeyPress -= GlobalHookKeyPress;

            m_GlobalHook.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Subscribe();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Unsubscribe();

            Thread thread = new Thread(this.RunMouseMovements) { IsBackground = true };
            Thread thread1 = new Thread(this.RunMouseClicks) { IsBackground = true };

            thread1.Start();
            thread.Start();

            startEvent.Set();
        }

        // Создаем события для синхронизации
        ManualResetEvent startEvent = new ManualResetEvent(false);

        // Метод для первого потока
        void RunMouseMovements()
        {
            startEvent.WaitOne(); // Ждем сигнала запуска
            this.mouseMovements.PlayMouseMovements();
        }

        // Метод для второго потока
        void RunMouseClicks()
        {
            startEvent.WaitOne(); // Ждем сигнала запуска
            this.mouseClicks.PlayMouseClicks();
        }

        private int GetDelay(ref int previousTime)
        {
            int currentTime = (int) this.stopWatch.ElapsedMilliseconds;

            int deltaTime = currentTime - previousTime;
            previousTime = currentTime;

            return deltaTime;
        }
    }
}