using System.Windows.Controls;
using System.Windows.Threading;

using RecBinderProject.Services;
using RecBinderProject.ViewModels;

namespace RecBinderProject
{
    public partial class ShowRecordPage : Page
    {
        private PlayRecordViewModel _playRecordViewModel;

        public ShowRecordPage(IRecordingRepository recordingRepository, string recordName)
        {
            InitializeComponent();

            this._playRecordViewModel = new PlayRecordViewModel(recordingRepository, recordName);
            this.DataContext = this._playRecordViewModel;
        }
    }
}