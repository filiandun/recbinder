using System.Windows.Controls;

using RecBinderProject.Models;
using RecBinderProject.ViewModels.RecordViewModels;


namespace RecBinderProject
{
    public partial class PlayRecordPage : Page
    {
        private readonly PlayRecordViewModel _playRecordViewModel;

        public PlayRecordPage(PlayRecordViewModel playRecordViewModel)
        {
            InitializeComponent();

            this._playRecordViewModel = playRecordViewModel;
            this.DataContext = this._playRecordViewModel;
        }
    }
}