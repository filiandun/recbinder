using System.Text.Json.Serialization;

using RecBinderProject.Models.InputEvents;


namespace RecBinderProject.Models
{
    public class Macro
    {
        public List<InputEvent> Events { get; }

        public MacroMetadata Metadata { get; }

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
        public void ClearEvents() => this.Events.Clear();
    }
}