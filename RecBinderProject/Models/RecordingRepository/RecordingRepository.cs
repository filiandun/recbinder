using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using RecBinderProject.Services.AppSettingsService;


namespace RecBinderProject.Models.RecordingRepository
{
    public class RecordingRepository : IRecordingRepository
    {
        public string _folderPath;

        public RecordingRepository(IAppSettingsService settingsService)
        {
            this._folderPath = settingsService.AppSettings.SaveDirectory;

            Directory.CreateDirectory(this._folderPath);
        }

        // TO DO
        // добавить, что если recordingInput => recordingInput != null && recordingInput.InputEvents != null && recordingInput.RecordingMetadata != null
        // оповестить пользователя, что имеются "битые" файлы записи
        public List<RecordingInput> LoadAll()
        {
            return Directory.EnumerateFiles(_folderPath, "*.json")
                            .Select(file => JsonSerializer.Deserialize<RecordingInput>(File.ReadAllText(file)))
                            .Where(recordingInput => recordingInput != null && recordingInput.InputEvents != null && recordingInput.RecordingMetadata != null)
                            .ToList()!;
        }

        public RecordingInput Read(string recordName)
        {
            string fileName = $"{SanitizeFileName(recordName)}.json";
            string filePath = Path.Combine(_folderPath, fileName);

            if (!File.Exists(filePath)) { throw new FileNotFoundException($"Файл записи, которую вы пытаетесь открыть - не существует, вероятно, он был удалён ({filePath})"); }

            RecordingInput? recordingInput = JsonSerializer.Deserialize<RecordingInput>(File.ReadAllText(filePath));

            if (recordingInput is null) { throw new FileNotFoundException($"Файл записи, которую вы пытаетесь открыть - не существует, вероятно, он был удалён ({filePath})"); }

            return recordingInput;
        }

        // TO DO
        // сделать проверку на то, что если файл уже существует с таким именем, то спросить пользователя, что с этим делать
        public void Save(RecordingInput recordingInput)
        {
            string fileName = $"{SanitizeFileName(recordingInput.RecordingMetadata.Name)}.json";
            string filePath = Path.Combine(_folderPath, fileName);
            string json = JsonSerializer.Serialize(recordingInput, new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            File.WriteAllText(filePath, json);
        }
       
        public void Delete(RecordingInput recordingInput)
        {
            string fileName = $"{SanitizeFileName(recordingInput.RecordingMetadata.Name)}.json";
            string filePath = Path.Combine(_folderPath, fileName);

            if (!File.Exists(filePath)) { throw new FileNotFoundException($"Файл записи, который вы пытаетесь удалить - не существует, вероятно, он уже был удалён ({filePath})"); }

            File.Delete(filePath);
        }

        private string SanitizeFileName(string name) => string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
    }
}
