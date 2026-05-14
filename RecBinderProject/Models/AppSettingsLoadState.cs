
namespace RecBinderProject.Models
{
    public enum AppSettingsLoadState
    {
        NotLoaded,
        LoadedFromFile, 

        MissingFile, 
        CorruptedFile, 
        AccessDenied, 
        Failed
    }
}
