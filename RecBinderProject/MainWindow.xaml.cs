using System.Diagnostics;
using System.Windows;

using ModernWpf.Controls;

using RecBinderProject.ViewModels;


namespace RecBinderProject
{
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; private set; }

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            this.ViewModel = viewModel;

            this.DataContext = viewModel;
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
                Debug.WriteLine($"tag: {navItem.Tag?.ToString()}");
                switch (navItem.Tag?.ToString())
                {
                    case "Recording": this.ViewModel.OpenRecording(); break;
                    //case "Help": this.ViewModel.OpenHelp(); break;
                    case "Settings": this.ViewModel.OpenSettings(); break;
                }
            }
        }
    }
}