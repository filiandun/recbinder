using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecBinderProject
{
    public class RecordingRepository
    {
        public string _folderPath;

        public RecordingRepository(string folderPath)
        {
            this._folderPath = folderPath;

            Directory.CreateDirectory(folderPath);
        }

        public List<RecordingInput> LoadAll()
        {
            return Directory.EnumerateFiles(this._folderPath, "*.json")
                            .Select(file => JsonSerializer.Deserialize<RecordingInput>(File.ReadAllText(file)))
                            .Where(ri => ri != null)
                            .ToList()!;
        }

        // TO DO
        // сделать проверку на то, что если файл уже существует с таким именем, то спросить пользователя, что с этим делать
        public void Save(RecordingInput recordingInput)
        {
            string fileName = $"{this.SanitizeFileName(recordingInput.RecordingMetadata.Name)}.json";
            string filePath = Path.Combine(this._folderPath, fileName);
            string json = JsonSerializer.Serialize<RecordingInput>(recordingInput, new JsonSerializerOptions() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            File.WriteAllText(filePath, json);
        }
       
        public void Delete(RecordingInput recordingInput)
        {
            string fileName = $"{this.SanitizeFileName(recordingInput.RecordingMetadata.Name)}.json";
            string filePath = Path.Combine(this._folderPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл, который вы пытаетесь удалить - не существует, вероятно, он уже был удалён ({filePath})");
            }

            File.Delete(filePath);
        }

        private string SanitizeFileName(string name) => string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
    }
}
