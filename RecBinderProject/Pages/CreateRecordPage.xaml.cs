using System.Windows.Controls;

using RecBinderProject.ViewModels;


namespace RecBinderProject
{
    public partial class CreateRecordPage : Page
    {
        private CreateRecordViewModel _viewModel;

        public CreateRecordPage()
        {
            InitializeComponent();

            this._viewModel = new CreateRecordViewModel();
            this.DataContext = this._viewModel;
        }
    }
}
