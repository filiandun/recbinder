using System.Windows;
using System.Windows.Input;

using ModernWpf.Controls;

using RecBinderProject.Models;
using RecBinderProject.ViewModels;
using RecBinderProject.Services.GlobalHotkeyService;
using RecBinderProject.Services.AppSettingsService;


namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        private readonly GlobalHotkeyService _globalHotkeyService;

        private readonly IAppSettingsService _appSettingsService;

        public MainWindowViewModel ViewModel { get; private set; }


        public MainWindow(MainWindowViewModel viewModel, GlobalHotkeyService globalHotkeyService, IAppSettingsService appSettingsService)
        {
            InitializeComponent();

            this.ViewModel = viewModel;
            this.DataContext = viewModel;

            this._globalHotkeyService = globalHotkeyService;
            this._appSettingsService = appSettingsService;

            this.SourceInitialized += this.OnSourceInitialized;
            this.ContentRendered += this.OnContentRendered;

            this.Closed += this.OnClosed;
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


        private void OnContentRendered(object? sender, EventArgs e)
        {
            string content = "";

            switch (this._appSettingsService.LoadState)
            {
                case AppSettingsLoadState.LoadedFromFile: content = "Настройки были загружены из файла"; break;

                case AppSettingsLoadState.MissingFile: content = "Файл настроек не найден, загружены дефолтные"; break;

                case AppSettingsLoadState.CorruptedFile: content = "Файл настроек повреждён, загруженые дефолтные"; break;

                case AppSettingsLoadState.AccessDenied: content = "Файл настроек недоступен, загруженые дефолтные"; break;

                case AppSettingsLoadState.Failed: throw new Exception();
            }

            ContentDialog contentDialog = new ContentDialog()
            {
                Content = content,
                DefaultButton = ContentDialogButton.Close,
                CloseButtonText = "Ok"
            };
            contentDialog.ShowAsync();
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


        private void OnClosed(object? sender, EventArgs e)
        {
            this._globalHotkeyService.HotkeyPressed -= this.ViewModel.OnHotkeyPressed;
            this._globalHotkeyService.Dispose();
        }
    }
}