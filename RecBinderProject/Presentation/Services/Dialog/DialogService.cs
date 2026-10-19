using System.Windows;

using RecBinderProject.Presentation.Views;
using RecBinderProject.Presentation.ViewModels.DialogWindow;


namespace RecBinderProject.Presentation.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public async Task<DialogResult> ShowAsync(string title, string message, string primaryButtonText = "OK", string? secondaryButtonText = null)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                return await Application.Current.Dispatcher.Invoke(() => this.ShowAsync(title, message, primaryButtonText, secondaryButtonText));
            } 
            else
            {
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

                    DialogResult finalResult = DialogResult.Primary;

                    DialogButton primaryButton = new DialogButton()
                    {
                        Text = primaryButtonText,
                        Action = () =>
                        {
                            finalResult = DialogResult.Primary;
                            dialogWindow.DialogResult = true;
                        }
                    };

                    DialogButton? secondaryButton = secondaryButtonText is null ? null : new DialogButton()
                    {
                        Text = secondaryButtonText,
                        Action = () =>
                        {
                            finalResult = DialogResult.Secondary;
                            dialogWindow.DialogResult = true;
                        }
                    };

                    dialogWindow.DataContext = new DialogWindowViewModel(title, message, primaryButton, secondaryButton);

                    dialogWindow.ShowDialog();

                    return finalResult;
                }
                else
                {
                    MessageBox.Show(message, title);

                    return DialogResult.Primary;
                }
            }
        }
    }
}
