using System.Windows;

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
    }
}