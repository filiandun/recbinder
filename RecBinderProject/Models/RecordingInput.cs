using RecBinderProject.Models.Inputs;
using System.Text.Json.Serialization;

namespace RecBinderProject.Models
{
    /// <summary>
    /// record - чтобы объект был immutable (для дальнейшего реализования undo/redo)
    /// </summary>
    public class RecordingInput
    {
        public RecordingMetadata RecordingMetadata { get; init; }
        public InputEvents InputEvents { get; init; }

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