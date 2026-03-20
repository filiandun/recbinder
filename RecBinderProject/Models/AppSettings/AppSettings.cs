
namespace RecBinderProject.Models.AppSettings
{
    public class AppSettings
    {
        public GeneralAppSettings General { get; set; }

        //public ProfilesAppSettings Proiles { get; set; }
        //public AdvancedAppSettings Advanced { get; set; }

        public AppSettings()
        {
            this.General = new GeneralAppSettings();
        }
    }
}
