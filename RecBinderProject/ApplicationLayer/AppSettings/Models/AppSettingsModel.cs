namespace RecBinderProject.ApplicationLayer.AppSettings.Models
{
    public class AppSettingsModel
    {
        public GeneralAppSettingsModel General { get; set; }

        //public ProfilesAppSettings Proiles { get; set; }
        //public AdvancedAppSettings Advanced { get; set; }

        public AppSettingsModel()
        {
            this.General = new GeneralAppSettingsModel();
        }
    }
}
