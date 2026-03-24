using System.Windows.Controls;
using RecBinderProject.ViewModels.MacroExecutionViewModels;


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
