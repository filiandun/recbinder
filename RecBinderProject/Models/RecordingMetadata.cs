using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RecBinderProject.Models
{
    public class RecordingMetadata
    {
        private string _name;
        private string _description;
        private TimeSpan _duration;
        private DateTime _creationDateTime;

        public string Name 
        {
            get => this._name;
            set
            {
                if (!RecordingMetadata.NameValidation(value))
                {
                    throw new ArgumentException("Имя неккоректно");
                }
                this._name = value;
            }
        }

        public string Description
        {
            get => this._description;
            set
            {
                if (!RecordingMetadata.DescriptionValidation(value))
                {
                    throw new ArgumentException("Описание неккоректно");
                }
                this._description = value;
            }
        }

        public TimeSpan Duration
        {
            get => this._duration;
            set
            {
                if (!RecordingMetadata.DurationValidation(value))
                {
                    throw new ArgumentException("Длительность неккоректна");
                }
                this._duration = value;
            }
        }

        public DateTime CreationDateTime
        {
            get => this._creationDateTime;
            set
            {
                if (!RecordingMetadata.CreationDateTimeValidation(value))
                {
                    throw new ArgumentException("Дата создания некорректна");
                }
                this._creationDateTime = value;
            }
        }

        public RecordingMetadata(string name, string description, TimeSpan duration, DateTime creationDateTime)
        {
            this.Name = name;
            this.Description = description;
            this.Duration = duration;
            this.CreationDateTime = creationDateTime;
        }


        public static bool NameValidation(string name)
        {
            Debug.WriteLine("\n START");
            if (!string.IsNullOrWhiteSpace(name))
            {
                //string pattern = @"^[^<>:""/\|?*]+$";
                string pattern = @"^[^<>""/\|?*]+$";
                Regex regex = new Regex(pattern);
                Debug.WriteLine("NOT NULL TRUE\n");

                if (regex.IsMatch(name))
                {
                    Debug.WriteLine("IS NOT MATCH TRUE\n");
                    return true;
                }
            }
            return false;
        }

        public static bool DescriptionValidation(string description)
        {
            return true;
        }

        public static bool DurationValidation(TimeSpan duration)
        {
            if (!duration.Equals(TimeSpan.Zero))
            {
                if (duration <= TimeSpan.MaxValue)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CreationDateTimeValidation(DateTime creationDateTime)
        {
            return true;
        }
    }
}
