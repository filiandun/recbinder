using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using RecBinderProject.ApplicationLayer.MacroExecutionService.Engine;
using RecBinderProject.Infrastructure.Services.GlobalHotkey;

using RecBinderProject.Presentation.ViewModels.MainWindow;


namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private readonly GlobalHotkeyService _globalHotkeyService;

        private readonly IInputListener _inputListener;

        public MainWindowViewModel ViewModel { get; private set; }


        public MainWindow(MainWindowViewModel viewModel, GlobalHotkeyService globalHotkeyService, IInputListener inputListener)
        {
            InitializeComponent();

            this.ViewModel = viewModel;
            this.DataContext = viewModel;

            this._globalHotkeyService = globalHotkeyService;

            this._inputListener = inputListener;

            this.SourceInitialized += this.OnSourceInitialized;
            this.ContentRendered += this.OnContentRendered;

            this.Closed += this.OnClosed;
        }


        private async void OnSourceInitialized(object? sender, EventArgs e)
        {
            this._globalHotkeyService.Initialize(this);

            this._globalHotkeyService.UpdateBindings(new[]
            {
                new HotkeyBinding(HotkeyAction.StartRecording, ModifierKeys.Control | ModifierKeys.Alt, Key.R),
                new HotkeyBinding(HotkeyAction.StopRecording, ModifierKeys.Control | ModifierKeys.Alt, Key.S),

                new HotkeyBinding(HotkeyAction.StartPlaying, ModifierKeys.Control | ModifierKeys.Alt, Key.P),
                new HotkeyBinding(HotkeyAction.StopPlaying, ModifierKeys.Control | ModifierKeys.Alt, Key.X),
            });

            this._globalHotkeyService.HotkeyPressed += this.ViewModel.OnHotkeyPressed;


            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            this._inputListener.Initialize(hwnd);
        }


        private async void OnContentRendered(object? sender, EventArgs e)
        {
            await this.ViewModel.InitializeAsync();
        }


        private void OnClosed(object? sender, EventArgs e)
        {
            this._globalHotkeyService.HotkeyPressed -= this.ViewModel.OnHotkeyPressed;
            this._globalHotkeyService.Dispose();
        }
    }
}