using System.Windows;
using System.Diagnostics;
using System.Windows.Input;

using ModernWpf.Controls;

using RecBinderProject.Models;
using RecBinderProject.ViewModels;
using RecBinderProject.Services.GlobalHotkeyService;


namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private readonly GlobalHotkeyService _globalHotkeyService;

        public MainWindowViewModel ViewModel { get; private set; }


        public MainWindow(MainWindowViewModel viewModel, GlobalHotkeyService globalHotkeyService)
        {
            InitializeComponent();

            this.ViewModel = viewModel;

            this.DataContext = viewModel;

            this._globalHotkeyService = globalHotkeyService;

            this.SourceInitialized += this.OnSourceInitialized;
            this.Closed += this.OnClosed;
        }


        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is MacroNavigationItemViewModel macroItem)
            {
                this.ViewModel.OpenPlayback(macroItem.Macro);
                return;
            }

            if (args.InvokedItemContainer is NavigationViewItem navItem)
            {
                switch (navItem.Tag?.ToString())
                {
                    case "Recording": this.ViewModel.OpenRecording(); break;
                    case "Help": this.ViewModel.OpenHelp(); break;
                    case "Settings": this.ViewModel.OpenSettings(); break;
                }
            }
        }

        private void OnSourceInitialized(object? sender, EventArgs e)
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
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            this._globalHotkeyService.HotkeyPressed -= this.ViewModel.OnHotkeyPressed;
            this._globalHotkeyService.Dispose();
        }
    }
}