using RecBinderProject.ViewModels.RecordViewModels;
using System.Diagnostics;
using System.Windows.Controls;

namespace RecBinderProject
{
    public partial class ShowRecordPage : Page
    {
        private readonly PlayRecordViewModel _playRecordViewModel;

        public ShowRecordPage(IRecordingRepository recordingRepository, string recordName)
        {
            this._playRecordViewModel = new PlayRecordViewModel(recordingRepository, recordName);
            this.DataContext = this._playRecordViewModel;

            InitializeComponent();
        }
    }
}