using System.Windows;

using RecBinderProject.Presentation.Views;
using RecBinderProject.Presentation.ViewModels.DialogWindow;


namespace RecBinderProject.Presentation.Services
{
    public class DialogService : IDialogService
    {
        public void Show(string title, string message)
        {
            Window mainWindow = Application.Current.MainWindow;

            DialogWindow dialogWindow = new()
            {
                Owner = mainWindow,

                Width = mainWindow.ActualWidth,
                Height = mainWindow.ActualHeight,

                Top = mainWindow.Top,
                Left = mainWindow.Left
            };

            dialogWindow.DataContext = new DialogWindowViewModel(title, message, () => dialogWindow.DialogResult = true);

            dialogWindow.ShowDialog();
        }
    }
}
