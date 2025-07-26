using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecBinderProject
{
    public class RecordingInput
    {
        public RecordingMetadata RecordingMetadata { get; set; }
        public InputEvents InputEvents { get; set; }

        public RecordingInput(RecordingMetadata recordingMetadata, InputEvents inputEvents)
        {
            this.RecordingMetadata = recordingMetadata;
            this.InputEvents = inputEvents;
        }
    }
}