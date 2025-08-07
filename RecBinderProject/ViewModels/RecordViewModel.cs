using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecBinderProject.ViewModels
{
    public abstract class RecordViewModel : INotifyPropertyChanged
    {
        public abstract event PropertyChangedEventHandler? PropertyChanged;
    }
}
