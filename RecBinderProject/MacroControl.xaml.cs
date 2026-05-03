using System.Windows.Controls;


namespace RecBinderProject
{
    public partial class MacroControl : UserControl
    {
        public string MacroName { get; set; }
        public string MacroDescription { get; set; }

        public MacroControl(string recordName, string recordDescription)
        {
            InitializeComponent();

            this.MacroName = recordName;
            this.MacroDescription = recordDescription;

            this.recordNameLabel.Content = recordName;
            this.recordDescriptionLabel.Content = recordDescription;
        }
    }
}
