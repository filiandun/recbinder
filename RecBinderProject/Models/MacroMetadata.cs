namespace RecBinderProject.Models
{
    public record RecordingMetadataSnapshot( // TODO for undo/redo
        string Name,
        string Description,
        TimeSpan Duration,
        DateTime CreationDateTime
    );

    public class MacroMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime CreationDateTime { get; set; }

        public MacroMetadata()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Duration = TimeSpan.Zero;
            this.CreationDateTime = DateTime.MinValue;
        }
        
        public MacroMetadata(string name, string description, TimeSpan duration, DateTime creationDateTime)
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

        public static MacroMetadata FromSnapshot(RecordingMetadataSnapshot snapshot)
        {
            return new MacroMetadata(snapshot.Name, snapshot.Description, snapshot.Duration, snapshot.CreationDateTime);
        }
    }
}
