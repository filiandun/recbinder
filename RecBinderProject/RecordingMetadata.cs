using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecBinderProject
{
    public class RecordingMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime CreationDateTime { get; set; }

        public RecordingMetadata(string name, string description, TimeSpan duration, DateTime creationDateTime)
        {
            this.Name = name;
            this.Description = description;
            this.Duration = duration;
            this.CreationDateTime = creationDateTime;
        }
    }
}
