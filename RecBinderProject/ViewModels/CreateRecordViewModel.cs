using RecBinderProject.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RecBinderProject.ViewModels
{
    internal class CreateRecordViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand StartRecordingCommand { get; }

        private CreateRecordService _createRecordService;

        public CreateRecordViewModel()
        {
            this._createRecordService = new CreateRecordService();
            this.StartRecordingCommand = new RelayCommand(StartRecording);
        }

        private string _statusText;
        public string StatusText
        {
            get => this._statusText;
            set
            {
                this._statusText = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusText)));
            }
        }

        private void StartRecording()
        {
            this._createRecordService.StartRecord();
            this.StatusText = "Идёт запись...";
            this.RaiseCanExecuteChanged();
        }

        private void HandleRecordingStopped()
        {
            this.StatusText = "Запись завершена";
            this.RaiseCanExecuteChanged();
        }

        private bool CanStartRecording()
        {
            return !this._createRecordService.IsRecording; // условие по твоей логике
        }

        private void RaiseCanExecuteChanged()
        {
            if (this.StartRecordingCommand is RelayCommand rc)
            {
                rc.RaiseCanExecuteChanged();
            }
        }
    }
}
}
