using System.Windows.Controls;

using RecBinderProject.ViewModels.RecordViewModels;


namespace RecBinderProject
{
    public partial class RecordingPage : Page
    {
        public RecordingPage(MacroRecordingViewModel recordingViewModel)
        {
            InitializeComponent();

            this.DataContext = recordingViewModel;
        }
    }
}
