using System.Windows.Controls;

using RecBinderProject.ViewModels;


namespace RecBinderProject
{
    public partial class CreateRecordPage : Page
    {
        private CreateRecordViewModel _viewModel;
        private readonly IRecordingRepository _recordingRepository;

        public CreateRecordPage(IRecordingRepository recordingRepository)
        {
            InitializeComponent();

            this._recordingRepository = recordingRepository;

            this._viewModel = new CreateRecordViewModel(this._recordingRepository);
            this.DataContext = this._viewModel;
        }
    }
}
