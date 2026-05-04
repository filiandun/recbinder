using System.ComponentModel;


namespace RecBinderProject.ViewModels.AppSettingsViewModels
{
    // TODO: сейчас интерфейс полурак полурыба, так как в нём есть методы, который подразумевают, что будет две модели данных (оригинальная и изменяемая),
    // однако для простых секций (уровня, когда только CheckBox есть) хватит только одного метода - ApplyToService.
    // Тут нужно делать древо наследования интерфейсов, типо ISectionVM с ApplyToService, а от него наследовать уже этот интерфейс.
    internal interface IAppSettingsSectionViewModel : INotifyPropertyChanged
    {
        public bool IsSaveAllowed { get; }
        public bool IsCancelAllowed { get; }
        public bool IsResetAllowed { get; }

        public bool HasEdits { get; }
        public bool HasErrors { get; }

        public void SaveLocal();
        public void ApplyToService();
        public void CancelLocal();
        public void ResetLocal();
    }
}
