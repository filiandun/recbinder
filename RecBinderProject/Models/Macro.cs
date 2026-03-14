using System.Text.Json.Serialization;

using RecBinderProject.Models.InputEvents;


namespace RecBinderProject.Models
{
    public class Macro
    {
        private readonly List<InputEvent> _events;

        public IReadOnlyList<InputEvent> Events => this._events;
        public MacroMetadata Metadata { get; }

        public Macro(MacroMetadata metadata)
        {
            this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this._events = new List<InputEvent>();
        }

        [JsonConstructor]
        public Macro(MacroMetadata metadata, List<InputEvent> events)
        {
            this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this._events = events ?? throw new ArgumentNullException(nameof(events));
        }

        public void AddEvent(InputEvent userEvent) => this._events.Add(userEvent);
        public void ClearEvents() => this._events.Clear();
    }
}