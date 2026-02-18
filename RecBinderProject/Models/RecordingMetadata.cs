
namespace RecBinderProject.Models
{
    public record RecordingMetadataSnapshot( // TODO for undo/redo
        string Name,
        string Description,
        TimeSpan Duration,
        DateTime CreationDateTime
    );

    public class RecordingMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime CreationDateTime { get; set; }

        public RecordingMetadata()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Duration = TimeSpan.Zero;
            this.CreationDateTime = DateTime.MinValue;
        }
        
        public RecordingMetadata(string name, string description, TimeSpan duration, DateTime creationDateTime)
        {
            this.Name = name;
            this.Description = description;
            this.Duration = duration;
            this.CreationDateTime = creationDateTime;
        }

        public RecordingMetadataSnapshot ToSnapshot()
        {
            return new RecordingMetadataSnapshot(this.Name, this.Description, this.Duration, this.CreationDateTime);
        }

        public static RecordingMetadata FromSnapshot(RecordingMetadataSnapshot snapshot)
        {
            return new RecordingMetadata(snapshot.Name, snapshot.Description, snapshot.Duration, snapshot.CreationDateTime);
        }
    }
}
