
namespace RecBinderProject.Domain
{
    public class MacroMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }

        public DateTime CreationDateTime { get; set; }

        public int ResolutionWidth { get; set; }
        public int ResolutionHeight { get; set; }

        
        public MacroMetadata(string name, string description, TimeSpan duration, DateTime creationDateTime, int resolutionWidth, int resolutionHeight)
        {
            this.Name = name;
            this.Description = description;
            this.Duration = duration;

            this.CreationDateTime = creationDateTime;

            this.ResolutionWidth = resolutionWidth;
            this.ResolutionHeight = resolutionHeight;
        }
    }
}
