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
        public string RecordName { get; set; }
        public string RecordDescription { get; set; }

        public RecordControl(string recordName, string recordDescription)
        {
            InitializeComponent();

            this.RecordName = recordName;
            this.RecordDescription = recordDescription;

            this.recordNameLabel.Content = recordName;
            this.recordDescriptionLabel.Content = recordDescription;
        }
    }
}
