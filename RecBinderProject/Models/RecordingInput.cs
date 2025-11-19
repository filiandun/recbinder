using RecBinderProject.Models.Inputs;
using System.Text.Json.Serialization;

namespace RecBinderProject.Models
{
    public class RecordingInput
    {
        public RecordingMetadata RecordingMetadata { get; }
        public InputEvents InputEvents { get; }

        public RecordingInput(RecordingMetadata recordingMetadata)
        {
            this.RecordingMetadata = recordingMetadata ?? throw new ArgumentNullException(nameof(recordingMetadata));
            this.InputEvents = new InputEvents();
        }

        [JsonConstructor]
        public RecordingInput(RecordingMetadata recordingMetadata, InputEvents inputEvents)
        {
            this.RecordingMetadata = recordingMetadata ?? throw new ArgumentNullException(nameof(recordingMetadata));
            this.InputEvents = inputEvents ?? throw new ArgumentNullException(nameof(inputEvents));
        }
    }
}