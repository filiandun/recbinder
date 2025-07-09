using System.Windows.Controls;

namespace RecBinderProject
{
    public partial class RecordPage : Page
    {
        private string recordName;
        private double recordDuration;
        private DateTime recordDateTime;
        private string recordDescription;

        public RecordPage(string recordName, double recordDuration, DateTime recordDateTime, string recordDescription)
        {
            InitializeComponent();

            this.recordName = recordName;
            this.recordDuration = recordDuration;
            this.recordDateTime = recordDateTime;
            this.recordDescription = recordDescription;

            this.recordNameTextBlock.Text = recordName;
            this.recordDurationTextBlock.Text = recordDuration.ToString();
            this.recordDateTimeTextBlock.Text = recordDateTime.ToString();
            this.recordDescriptionTextBlock.Text = recordDescription;
        }
    }
}
