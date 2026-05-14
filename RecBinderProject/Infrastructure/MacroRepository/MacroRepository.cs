using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

using RecBinderProject.Domain;
using RecBinderProject.Infrastructure.AppSettingsService;


namespace RecBinderProject.Infrastructure.MacroRepository
{
    public class MacroRepository : IMacroRepository
    {
        private readonly ILogger<MacroRepository> _logger;

        private readonly IAppSettingsService _settingsService;

        private string FolderPath => this._settingsService.AppSettings.General.SaveDirectory;

        public MacroRepository(ILogger<MacroRepository> logger, IAppSettingsService settingsService)
        {
            this._logger = logger;

            this._settingsService = settingsService;

            Directory.CreateDirectory(this.FolderPath);
        }

        public MacroLoadResult LoadAll()
        {
            MacroLoadResult macroLoadResult = new();

            foreach (string filePath in Directory.EnumerateFiles(this.FolderPath, "*.json"))
            {
                try
                {
                    using FileStream fileStream = File.OpenRead(filePath);
                    Macro? macro = JsonSerializer.Deserialize<Macro>(fileStream);

                    if (macro is not null && macro.Events is not null && macro.Metadata is not null)
                    {
                        macroLoadResult.Macros.Add(macro);
                    }
                    else
                    {
                        macroLoadResult.FailedFiles.Add(filePath);
                    }
                }
                catch (Exception ex)
                {
                    macroLoadResult.FailedFiles.Add(filePath);
                    this._logger.LogError(ex, $"file cannot be loaded: {filePath}");
                }
            }

            return macroLoadResult;
        }

        public Macro Read(string macroName)
        {
            string fileName = $"{MacroRepository.SanitizeFileName(macroName)}.json";
            string filePath = Path.Combine(this.FolderPath, fileName);

            if (!File.Exists(filePath)) throw new FileNotFoundException($"read: file not found {filePath}");

            try
            {
                using FileStream fileStream = File.OpenRead(filePath);
                Macro? macro = JsonSerializer.Deserialize<Macro>(fileStream);

                if (macro is null) throw new InvalidDataException($"read: file is empty or corrupted {filePath}");

                return macro;
            }
            catch (JsonException je)
            {
                throw new InvalidDataException($"read: file is corrupted {filePath}", je);
            }
        }

        public void Save(Macro recordingInput, bool allowOverwrite = false)
        {
            string fileName = $"{MacroRepository.SanitizeFileName(recordingInput.Metadata.Name)}.json";
            string filePath = Path.Combine(this.FolderPath, fileName);

            //if (!allowOverwrite && File.Exists(filePath)) throw new InvalidOperationException($"save: file already exist {filePath}");

            string json = JsonSerializer.Serialize(recordingInput, new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            File.WriteAllText(filePath, json);
        }
       
        public void Delete(Macro recordingInput)
        {
            string fileName = $"{MacroRepository.SanitizeFileName(recordingInput.Metadata.Name)}.json";
            string filePath = Path.Combine(this.FolderPath, fileName);

            if (!File.Exists(filePath)) throw new FileNotFoundException($"delete: file not found {filePath}");

            File.Delete(filePath);
        }

        private static string SanitizeFileName(string name) => string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
    }
}
