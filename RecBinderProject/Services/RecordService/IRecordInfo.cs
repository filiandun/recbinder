using System.Runtime.CompilerServices;

using RecBinderProject.Models;


namespace RecBinderProject.Services.RecordService
{
    public enum PlaybackState
    {
        Playing,
        Paused,
        Stopped,
        Completed
    }

    public interface IRecordInfo
    {
        public event Action<PlaybackState>? StateChanged;

        public PlaybackState State { get; }

        public TimeSpan TotalTime { get; }
        public TimeSpan ElapsedTime { get; }
    }

    public interface IRecordProcess : IRecordInfo
    {
        public void Start();
        public void Pause();
        public void Resume();
        public void Stop();
    }

    public interface IRecordingMetadataSource
    {
        public RecordingMetadata RecordingMetadata { get; }
    }

    public abstract class BaseRecordService : IRecordProcess, IRecordingMetadataSource
    {
        public event Action<PlaybackState>? StateChanged;

        protected RecordOptions _recordOptions;

        protected PlaybackState _state;

        public PlaybackState State
        {
            get => this._state;
            set
            {
                this.Set(ref this._state, value);
            }
        }

        public abstract TimeSpan TotalTime { get; }
        public abstract TimeSpan ElapsedTime { get; }
        public abstract RecordingMetadata RecordingMetadata { get; }

        public BaseRecordService(RecordOptions recordOptions)
        {
            this._recordOptions = recordOptions;

            this.State = PlaybackState.Stopped;
        }

        private void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                this.StateChanged?.Invoke(this._state);
            }
        }

        public abstract void Start();
        public abstract void Pause();
        public abstract void Resume();
        public abstract void Stop();
    }
}
