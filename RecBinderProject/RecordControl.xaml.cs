using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RecBinderProject
{
    public partial class RecordControl : UserControl
    {
        public RecordingInput RecordingInput { get; set; }

        public RecordControl(RecordingInput recordingInput)
        {
            InitializeComponent();

            this.RecordingInput = recordingInput;

            this.recordNameLabel.Content = this.RecordingInput.RecordingMetadata.Name;
            this.recordDescriptionLabel.Content = this.RecordingInput.RecordingMetadata.Description;
        }
    }
}
