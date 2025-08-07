using RecBinderProject.Models.Inputs;

namespace RecBinderProject.Models
{
    public class RecordingInput
    {
        public RecordingMetadata RecordingMetadata { get; set; }
        public InputEvents InputEvents { get; set; }

        public RecordingInput(RecordingMetadata recordingMetadata, InputEvents inputEvents)
        {
            RecordingMetadata = recordingMetadata;
            InputEvents = inputEvents;
        }
    }
}