using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

using RecBinderProject.Domain;
using RecBinderProject.Infrastructure.Services.AppSettings;


namespace RecBinderProject.Infrastructure.Repositories
{
    public class MacroRepository : IMacroRepository
    {
        public event Action? MacroSaved;

        private readonly ILogger<MacroRepository> _logger;

        private readonly IAppSettingsService _settingsService;

        private string _saveDirectory => this._settingsService.AppSettings.General.SaveDirectory;


        public MacroRepository(ILogger<MacroRepository> logger, IAppSettingsService settingsService)
        {
            this._logger = logger;

            this._settingsService = settingsService;

            Directory.CreateDirectory(this._saveDirectory); // TODO тихо падает, если не удаётся создать директорию - непорядок
        }


        public MacroLoadResult LoadAll()
        {
            MacroLoadResult macroLoadResult = new();

            foreach (string filePath in Directory.EnumerateFiles(this._saveDirectory, "*.json"))
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
                    this._logger.LogError(ex, "file cannot be loaded: {0}", filePath);
                }
            }

            return macroLoadResult;
        }

        public Macro Read(string macroName)
        {
            string fileName = $"{MacroRepository.SanitizeFileName(macroName)}.json";
            string filePath = Path.Combine(this._saveDirectory, fileName);

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

        public void Save(Macro macro, bool allowOverwrite = false)
        {
            string fileName = $"{MacroRepository.SanitizeFileName(macro.Metadata.Name)}.json";
            string filePath = Path.Combine(this._saveDirectory, fileName);

            // TODO раскомментировать и добавить try-обработку в месте, где вызывается
            //if (!allowOverwrite && File.Exists(filePath)) throw new InvalidOperationException($"save: file already exist {filePath}");

            string json = JsonSerializer.Serialize(macro, new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            File.WriteAllText(filePath, json);

            this.MacroSaved?.Invoke();
        }
       
        public void Delete(Macro macro)
        {
            string fileName = $"{MacroRepository.SanitizeFileName(macro.Metadata.Name)}.json";
            string filePath = Path.Combine(this._saveDirectory, fileName);

            if (!File.Exists(filePath)) throw new FileNotFoundException($"delete: file not found {filePath}");

            File.Delete(filePath);
        }

        private static string SanitizeFileName(string name) => string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
    }
}
