namespace RecBinderProject.Infrastructure.Services.AppSettings
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
