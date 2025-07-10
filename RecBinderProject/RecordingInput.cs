using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecBinderProject
{
    public class RecordingInput
    {
        public InputEvents InputEvents { get; private set; }
        public RecordingMetadata RecordingMetadata {  get; private set; }

        public RecordingInput(InputEvents inputEvents, RecordingMetadata recordingMetadata)
        {
            this.InputEvents = inputEvents;
            this.RecordingMetadata = recordingMetadata;
        }
    }
}
