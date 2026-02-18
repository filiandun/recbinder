using System.Windows.Controls;

using RecBinderProject.ViewModels.RecordViewModels;


namespace RecBinderProject
{
    public partial class CreateRecordPage : Page
    {
        private readonly CreateRecordViewModel _createRecordViewModel;

        public CreateRecordPage(CreateRecordViewModel createRecordViewModel)
        {
            InitializeComponent();

            this._createRecordViewModel = createRecordViewModel;
            this.DataContext = this._createRecordViewModel;
        }
    }
}
