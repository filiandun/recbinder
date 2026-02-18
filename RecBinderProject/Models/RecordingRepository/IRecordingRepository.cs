
namespace RecBinderProject.Models.RecordingRepository
{
    public interface IRecordingRepository
    {
        public List<RecordingInput> LoadAll();

        public RecordingInput Read(string recordName);
        public void Save(RecordingInput recordingInput);

        public void Delete(RecordingInput recordingInput);
    }
}
