using System.Windows.Input;

using RecBinderProject.Presentation.Commands;
using RecBinderProject.Presentation.Services.Dialog;


namespace RecBinderProject.Presentation.ViewModels.DialogWindow
{
    public class DialogWindowViewModel
    {
        public string Title { get; private set; }
        public string Message { get; private set; }

        public DialogButton PrimaryButton { get; private set; }
        public DialogButton? SecondaryButton { get; private set; }


        public ICommand PrimaryCommand { get; private set; }
        public ICommand? SecondaryCommand { get; private set; }


        public DialogWindowViewModel(string title, string message, DialogButton primaryButton, DialogButton? secondaryButton)
        {
            this.Title = title;
            this.Message = message;

            this.PrimaryButton = primaryButton;
            this.SecondaryButton = secondaryButton;

            this.PrimaryCommand = new RelayCommand(this.PrimaryButton.Action);

            if (this.SecondaryButton is not null)
            {
                this.SecondaryCommand = new RelayCommand(this.SecondaryButton.Action);
            }
        }
    }
}
