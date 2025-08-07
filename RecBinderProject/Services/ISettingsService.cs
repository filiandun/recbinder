
using RecBinderProject.Models;

namespace RecBinderProject.Services
{
    public interface ISettingsService
    {
        public AppSettings AppSettings { get; set; }

        public void Load();
        public void Save();
    }
}
