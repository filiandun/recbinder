using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using RecBinderProject.Domain;


namespace RecBinderProject.Presentation.ViewModels.MacroMetadataExecution
{
    enum MacroDurationUnit
    {
        Milliseconds,
        Seconds,
        Minutes,
        Unlimited
    }


    public class MacroMetadataCreatorViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private readonly Dictionary<string, List<string>> _errors;

        private readonly MacroMetadata _macroMetadataModel;

        private string _macroName;
        private string _macroDescription;
        private TimeSpan _macroDuration;
        private int _macroDurationUnitIndex; // TO DO создать enum, чтобы магические числа не использовать
        private DateTime _macroCreationDateTime;

        public MacroMetadata MacroMetadataModel => this._macroMetadataModel;    // TODO думаю, что лучше убрать свойство и сделать возврат модели только в ApplyChanges
                                                                                // хотя, может и не надо, данные некорректные всё равно в ней не будут
                                                                                // они могут только отличаться от тех, что последние пользователь ввёл

        public bool HasErrors => this._errors.Count > 0;

        public string MacroName
        {
            get => this._macroName;
            set
            {
                this.Set(ref this._macroName, value);
                this.ValidateMacroName();
            }
        }

        public string MacroDescription
        {
            get => this._macroDescription;
            set => this.Set(ref this._macroDescription, value);
        }

        public int MacroDurationValue
        {
            get
            {
                return this._macroDurationUnitIndex switch
                {
                    0 => this._macroDuration.Milliseconds,
                    1 => this._macroDuration.Seconds,
                    2 => this._macroDuration.Minutes,
                    _ => 0
                };
            }

            set
            {
                TimeSpan newMacroDuration = TimeSpan.MaxValue;
                if (value > 0)
                {
                    newMacroDuration = this._macroDurationUnitIndex switch
                    {
                        0 => TimeSpan.FromMilliseconds(value),
                        1 => TimeSpan.FromSeconds(value),
                        2 => TimeSpan.FromMinutes(value),
                        _ => TimeSpan.MaxValue,
                    };
                }

                this.Set(ref this._macroDuration, newMacroDuration);
            }
        }

        public int MacroDurationUnitIndex
        {
            get => this._macroDurationUnitIndex;
            set => this.Set(ref this._macroDurationUnitIndex, value);
        }


        public MacroMetadataCreatorViewModel()
        {
            this._errors = new Dictionary<string, List<string>>();

            this._macroName = "RecBinder_" + DateTime.Now.ToString("G");
            this._macroDescription = string.Empty;
            this._macroCreationDateTime = DateTime.Now;

            this._macroMetadataModel = new MacroMetadata(this._macroName, this._macroDescription, TimeSpan.MaxValue, DateTime.Now);
        }


        public void ApplyChanges()
        {
            if (this.HasErrors) throw new InvalidOperationException("Невозможно применить изменения, так как имеются ошибки валидации.");

            this._macroMetadataModel.Name = this._macroName;
            this._macroMetadataModel.Description = this._macroDescription;
            this._macroMetadataModel.Duration = this._macroDuration;
            this._macroMetadataModel.CreationDateTime = this._macroCreationDateTime;
        }


        protected void ValidateMacroName()
        {
            if (!string.IsNullOrWhiteSpace(this.MacroName))
            {
                this.ClearErrors(nameof(this.MacroName));
            }
            else
            {
                this.AddError(nameof(this.MacroName), "Имя некорректно");
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
