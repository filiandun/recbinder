using System.Windows;

using RecBinderProject.Presentation.Views;
using RecBinderProject.Presentation.ViewModels.DialogWindow;


namespace RecBinderProject.Presentation.Services
{
    public class DialogService : IDialogService
    {
        public void Show(string title, string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => this.Show(title, message));
                return;
            }

            Window? mainWindow = Application.Current.MainWindow;

            if (mainWindow != null && mainWindow.IsLoaded && mainWindow.IsVisible)
            {
                DialogWindow dialogWindow = new()
                {
                    Owner = mainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,

                    Top = mainWindow!.Top,
                    Left = mainWindow.Left,

                    Width = mainWindow.ActualWidth,
                    Height = mainWindow.ActualHeight
                };

                dialogWindow.DataContext = new DialogWindowViewModel(title, message, () => dialogWindow.DialogResult = true);

                dialogWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show(message, title);
            }
        }
    }
}
