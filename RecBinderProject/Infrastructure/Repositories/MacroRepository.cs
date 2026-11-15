using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

using RecBinderProject.Domain;
using RecBinderProject.ApplicationLayer.MacroRepository;
using RecBinderProject.ApplicationLayer.AppSettings;


namespace RecBinderProject.Infrastructure.Repositories
{
    public class MacroRepository : IMacroRepository
    {
        public event Action<Macro>? MacroSaved;

        private readonly ILogger<MacroRepository> _logger;

        private readonly IAppSettingsService _settingsService;

        private readonly JsonSerializerOptions _saveJsonSerializerOptions;

        private string _saveDirectory => this._settingsService.AppSettings.General.SaveDirectory;


        public MacroRepository(ILogger<MacroRepository> logger, IAppSettingsService settingsService)
        {
            this._logger = logger;

            this._settingsService = settingsService;

            this._saveJsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        }


        public MacroLoadResult LoadAll()
        {
            MacroLoadResult macroLoadResult = new MacroLoadResult();

            if (!Directory.Exists(this._saveDirectory)) return macroLoadResult;

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
                    this._logger.LogError(ex, "File cannot be loaded: {FilePath}", filePath);
                }
            }

            return macroLoadResult;
        }


        public Macro Read(string macroName)
        {
            string filePath = this.GetFilePath(macroName);

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

        public async Task SaveAsync(Macro macro)
        {
            Directory.CreateDirectory(this._saveDirectory);

            string filePath = this.GetUniqueFilePath(macro);

            using FileStream fileStream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(fileStream, macro, this._saveJsonSerializerOptions);

            this.MacroSaved?.Invoke(macro);
        }
       
        public void Delete(Macro macro)
        {
            string filePath = this.GetFilePath(macro.Metadata.Name);

            if (!File.Exists(filePath)) throw new FileNotFoundException($"delete: file not found {filePath}");

            File.Delete(filePath);
        }


        private string GetUniqueFilePath(Macro macro)
        {
            string filePath = this.GetFilePath(macro.Metadata.Name);

            if (File.Exists(filePath))
            {
                int counter = 1;
                string newMacroName;
                
                do
                {
                    newMacroName = $"{macro.Metadata.Name} ({counter})";
                    filePath = this.GetFilePath(newMacroName);

                    counter++;
                } while (File.Exists(filePath));

                macro.Metadata.Name = newMacroName; // TODO плохо, что тут меняется, это не дело
            }

            return filePath;
        }

        private string GetFilePath(string macroName)
        {
            string fileName = $"{SanitizeFileName(macroName)}.json";
            string filePath = Path.Combine(this._saveDirectory, fileName);

            return filePath;
        }

        private static string SanitizeFileName(string name) => string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
    }
}
