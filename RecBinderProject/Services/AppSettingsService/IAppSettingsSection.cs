
using System.ComponentModel;

namespace RecBinderProject.Services.AppSettingsService
{
    internal interface IAppSettingsSection : INotifyPropertyChanged
    {
        public bool IsSaveAllowed { get; }
        public bool IsCancelAllowed { get; }
        public bool IsResetAllowed { get; }

        public bool HasEdits { get; }
        public bool HasErrors { get; }


        public void Save();
        public void Cancel();
        public void Reset();
    }
}
