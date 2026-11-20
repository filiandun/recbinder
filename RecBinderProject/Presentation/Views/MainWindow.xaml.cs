using RecBinderProject.ApplicationLayer.GlobalHotkey;
using RecBinderProject.Infrastructure.Input;
using RecBinderProject.Infrastructure.Services.GlobalHotkey;
using RecBinderProject.Presentation.ViewModels.MainWindow;

using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;


namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private readonly IGlobalHotkeyService _globalHotkeyService;
        private readonly IInputListener _inputListener;

        public MainWindowViewModel ViewModel { get; private set; }


        public MainWindow(MainWindowViewModel viewModel, IGlobalHotkeyService globalHotkeyService, IInputListener inputListener)
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
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            this._globalHotkeyService.Initialize(hwnd);

            this._globalHotkeyService.UpdateBindings(new[]
            {
                new HotkeyBinding(HotkeyAction.StartRecording, ModifierKeys.Control | ModifierKeys.Alt, Key.R),
                new HotkeyBinding(HotkeyAction.StopRecording, ModifierKeys.Control | ModifierKeys.Alt, Key.S),

                new HotkeyBinding(HotkeyAction.StartPlaying, ModifierKeys.Control | ModifierKeys.Alt, Key.P),
                new HotkeyBinding(HotkeyAction.StopPlaying, ModifierKeys.Control | ModifierKeys.Alt, Key.X),
            });

            this._globalHotkeyService.HotkeyPressed += this.ViewModel.OnHotkeyPressed;


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