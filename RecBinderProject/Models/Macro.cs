using System.Text.Json.Serialization;

using RecBinderProject.Models.InputEvents;


namespace RecBinderProject.Models
{
    public class Macro
    {
        public List<InputEvent> Events { get; private set; } // TODO в идеале, тут должен быть IReadOnlyList, но и-за json сериализации я не смог сделать так, чтобы он при этом не выдавал ошибок
        public MacroMetadata Metadata { get; private set; }

        public Macro(MacroMetadata metadata)
        {
            this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this.Events = new List<InputEvent>();
        }

        [JsonConstructor]
        public Macro(MacroMetadata metadata, List<InputEvent> events)
        {
            this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this.Events = events ?? throw new ArgumentNullException(nameof(events));
        }

        public void AddEvent(InputEvent userEvent) => this.Events.Add(userEvent);

        public void UpdateMetadata(MacroMetadata metadata)
        {
            this.Metadata = new MacroMetadata
            {
                Name = metadata.Name,
                Description = metadata.Description,
                Duration = metadata.Duration,
                CreationDateTime = metadata.CreationDateTime
            };
        }
    }
}