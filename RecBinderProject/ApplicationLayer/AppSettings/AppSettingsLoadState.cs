namespace RecBinderProject.ApplicationLayer.AppSettings
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
