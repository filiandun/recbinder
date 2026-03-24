using System.Windows.Controls;

using RecBinderProject.Models;
using RecBinderProject.ViewModels.MacroExecutionViewModels;

namespace RecBinderProject
{
    public partial class PlaybackPage : Page
    {
        public PlaybackPage(MacroPlaybackViewModel playbackViewModel)
        {
            InitializeComponent();

            this.DataContext = playbackViewModel;
        }
    }
}