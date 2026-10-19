namespace RecBinderProject.Presentation.Services.Dialog
{
    public interface IDialogService
    {
        public Task<DialogResult> ShowAsync(string title, string message, string primaryButtonText = "OK", string? secondaryButtonText = null);
    }
}
