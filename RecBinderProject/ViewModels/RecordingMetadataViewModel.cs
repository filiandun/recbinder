using RecBinderProject.Models;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RecBinderProject.ViewModels
{
    public class RecordingMetadataViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private readonly Dictionary<string, List<string>> _errors;
        private RecordingMetadata _recordingMetadataModel; // TO DO существование этого поля под вопросом

        private string _recordName;
        private string _recordDescription;
        private double _recordDurationValue;
        private int _recordDurationUnitIndex;

        public bool HasErrors => this._errors.Any();

        public string RecordName
        {
            get => this._recordName;
            set
            {
                this.Set(ref this._recordName, value);
                this.ValidateRecordName();
            }
        }
        public string RecordDescription
        {
            get => this._recordDescription;
            set
            {
                this.Set(ref this._recordDescription, value);
            }
        }
        public double RecordDurationValue
        {
            get => this._recordDurationValue;
            set
            {
                this.Set(ref this._recordDurationValue, value);
            }
        }
        public int RecordDurationUnitIndex
        {
            get => this._recordDurationUnitIndex;
            set
            {
                this.Set(ref this._recordDurationUnitIndex, value);
            }
        }

        public RecordingMetadataViewModel()
        {
            this._errors = new Dictionary<string, List<string>>();

            this._recordName = "RecBinder_" + DateTime.Now.ToString("G");
            this._recordDescription = "";
            this._recordingMetadataModel = new RecordingMetadata(this._recordName, this._recordDescription, TimeSpan.MaxValue, DateTime.Now);
        }

        public RecordingMetadata ToModel()
        {
            try
            {
                TimeSpan recordDuration = this._recordDurationValue > 0 ? this._recordDurationUnitIndex switch
                {
                    0 => TimeSpan.FromMilliseconds(this._recordDurationValue),
                    1 => TimeSpan.FromSeconds(this._recordDurationValue),
                    2 => TimeSpan.FromMinutes(this._recordDurationValue),
                    _ => TimeSpan.MaxValue
                } : TimeSpan.MaxValue;

                // string recordName = string.IsNullOrWhiteSpace(this._recordName) ? $"RecBinder_{DateTime.Now:g}" : this._recordName; // это должно быть во время сохранения, а не тут

                RecordingMetadata recordingMetadata = new RecordingMetadata(this.RecordName, this.RecordDescription, recordDuration, DateTime.Now);

                return recordingMetadata;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void FromModel(RecordingMetadata recordingMetadata)
        {
            this.RecordName = recordingMetadata.Name;
            this.RecordDescription = recordingMetadata.Description;
            this.RecordDurationValue = recordingMetadata.Duration.TotalMilliseconds;
        }


        public IEnumerable GetErrors(string? propertyName)
        {
            if (this._errors.ContainsKey(propertyName))
            {
                return this._errors[propertyName];
            }

            return null;
        }

        private void ValidateRecordName()
        {
            if (RecordingMetadata.NameValidation(this.RecordName))
            {
                this.ClearErrors(nameof(this.RecordName));
            }
            else
            {
                this.AddError(nameof(this.RecordName), "Имя некорректно");
            }
        }

        private void AddError(string propertyName, string error)
        {
            if (!this._errors.ContainsKey(propertyName))
            {
                this._errors[propertyName] = new List<string>();
            }
            if (!this._errors[propertyName].Contains(error))
            {
                this._errors[propertyName].Add(error);
                this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (this._errors.ContainsKey(propertyName))
            {
                this._errors.Remove(propertyName);
                this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }


        private void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
