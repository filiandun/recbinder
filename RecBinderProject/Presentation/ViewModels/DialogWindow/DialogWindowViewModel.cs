using System.Windows.Input;

using RecBinderProject.Presentation.Commands;


namespace RecBinderProject.Presentation.ViewModels.DialogWindow
{
    public class DialogWindowViewModel
    {
        public string Title { get; private set; }
        public string Message { get; private set; }

        public ICommand CloseCommand { get; private set; }


        public DialogWindowViewModel(string title, string message, Action closeAction)
        {
            this.Title = title;
            this.Message = message;

            this.CloseCommand = new RelayCommand(closeAction);
        }
    }
}
