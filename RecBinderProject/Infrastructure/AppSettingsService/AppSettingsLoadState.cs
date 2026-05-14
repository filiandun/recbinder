namespace RecBinderProject.Infrastructure.AppSettingsService
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
