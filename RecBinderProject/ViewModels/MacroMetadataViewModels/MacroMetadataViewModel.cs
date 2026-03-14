using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using RecBinderProject.Models;


namespace RecBinderProject.ViewModels.MacroMetadataViewModels
{
    enum RecordDurationUnit
    {
        Milliseconds,
        Seconds,
        Minutes,
        Unlimited
    }

    // TODO пересмотреть архитектуру.
    // В текущем исполнении можно обращаться что к set, get полей так и к модели recordingMetadataModel самой:
    // из этого вытекает то, что свойство тут изменили, а саму модель recordingMetadataModel никто менять и не заставляет, соответсвенно, в модели свойства не валидировались.
    public class MacroMetadataViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private readonly Dictionary<string, List<string>> _errors;

        private string _recordName;
        private string _recordDescription;
        private TimeSpan _recordDuration;
        private int _recordDurationUnitIndex; // TO DO создать enum, чтобы магические числа не использовать
        private DateTime _recordCreationDateTime;

        private readonly MacroMetadata _recordingMetadataModel;
        public MacroMetadata RecordingMetadataModel => this._recordingMetadataModel;

        public bool HasErrors => this._errors.Count > 0;

        public int RecordDurationUnitIndex
        {
            get => this._recordDurationUnitIndex;
            set => this.Set(ref this._recordDurationUnitIndex, value);
        }

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
            set => this.Set(ref this._recordDescription, value);
        }

        public TimeSpan RecordDuration
        {
            get => this._recordDuration;
            set => this.Set(ref this._recordDuration, value);
        }

        public int RecordDurationValue
        {
            get
            {
                return this._recordDurationUnitIndex switch
                {
                    0 => this.RecordDuration.Milliseconds,
                    1 => this.RecordDuration.Seconds,
                    2 => this.RecordDuration.Minutes,
                    _ => 0
                };
            }

            set
            {
                TimeSpan newRecordDuration = TimeSpan.MaxValue;
                if (value > 0)
                {
                    newRecordDuration = this._recordDurationUnitIndex switch
                    {
                        0 => TimeSpan.FromMilliseconds(value),
                        1 => TimeSpan.FromSeconds(value),
                        2 => TimeSpan.FromMinutes(value),
                        _ => TimeSpan.MaxValue,
                    };
                }

                this.Set(ref this._recordDuration, newRecordDuration);
            }
        }

        public DateTime RecordCreationDateTime
        {
            get => this._recordCreationDateTime;
            set => this.Set(ref this._recordCreationDateTime, value);
        }

        public string RecordDurationStr
        {
            get => this._recordDuration.ToString(@"hh\:mm\:ss\:ff");
        }

        public string RecordCreationDateTimeStr // TO DO не будет работать, так как в методе Set, который выше, нет вызова PropertyChanged для этого свойства. На самом деле, работает, почему - не знаю
        {
            get => this._recordCreationDateTime.ToString("G");
        }

        public MacroMetadataViewModel()
        {
            this._errors = new Dictionary<string, List<string>>();

            this._recordName = "RecBinder_" + DateTime.Now.ToString("G");
            this._recordDescription = string.Empty;
            this._recordCreationDateTime = DateTime.Now;

            this._recordingMetadataModel = new MacroMetadata(this._recordName, this._recordDescription, TimeSpan.MaxValue, DateTime.Now);
        }

        public MacroMetadataViewModel(MacroMetadata recordingMetadata) // TODO плохой вариант будто бы
        {
            this._errors = new Dictionary<string, List<string>>();

            this._recordName = recordingMetadata.Name;
            this._recordDescription = recordingMetadata.Description;
            this._recordDuration = recordingMetadata.Duration;
            this._recordCreationDateTime = recordingMetadata.CreationDateTime;

            this._recordingMetadataModel = recordingMetadata;
        }

        public void ApplyChanges()
        {
            if (this.HasErrors) throw new InvalidOperationException("Невозможно применить изменения, так как имеются ошибки валидации.");

            this._recordingMetadataModel.Name = this._recordName;
            this._recordingMetadataModel.Description = this._recordDescription;
            this._recordingMetadataModel.Duration = this._recordDuration;
            this._recordingMetadataModel.CreationDateTime = this._recordCreationDateTime;
        }


        protected void ValidateRecordName()
        {
            if (!string.IsNullOrWhiteSpace(this.RecordName))
            {
                this.ClearErrors(nameof(this.RecordName));
            }
            else
            {
                this.AddError(nameof(this.RecordName), "Имя некорректно");
            }
        }


        public IEnumerable GetErrors(string? propertyName)
        {
            if (this._errors.ContainsKey(propertyName ?? throw new NullReferenceException()))
            {
                return this._errors[propertyName];
            }

            return Array.Empty<string>();
        }

        protected void AddError(string propertyName, string error)
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

        protected void ClearErrors(string propertyName)
        {
            if (this._errors.ContainsKey(propertyName))
            {
                this._errors.Remove(propertyName);
                this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }



        protected void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
