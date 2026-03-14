using System.Diagnostics;
using ModernWpf.Controls;

using RecBinderProject.Models;
using RecBinderProject.Models.MacroRepository;
using RecBinderProject.Services.RecordService;
using RecBinderProject.Services.LocalizationService;
using RecBinderProject.ViewModels.MacroMetadataViewModels;


namespace RecBinderProject.ViewModels.RecordViewModels
{
    public class MacroRecordingViewModel : BaseRecordViewModel
    {
        public MacroRecordingViewModel(IMacroRepository recordingRepository, IRecordProcess recordProcess, ILocalizationService localizationService, string localizationPrefix) : base(recordProcess, localizationService, localizationPrefix)
        {
            this._recordingMetadataViewModel = new MacroMetadataViewModel();

            this._recordOptions = new MacroOptions();
        }

        protected override void Start()
        {
            try
            {
                if (this._recordProcess.State == PlaybackState.Paused)
                {
                    this._recordProcess.Resume();
                }
                else
                {
                    this.RecordingMetadataViewModel.ApplyChanges();
                    MacroMetadata recordingMetadata = this.RecordingMetadataViewModel.RecordingMetadataModel;
                    Debug.WriteLine($"metadata:\n\tname: {recordingMetadata.Name}\n\tdecription: {recordingMetadata.Description}\n\tduration {recordingMetadata.Duration}\n\tdate {recordingMetadata.CreationDateTime}");

                    (this._recordProcess as MacroRecordService).UpdateMetadata(recordingMetadata); // TODO костыль
                    this._recordProcess.Start();
                }
            }
            catch (ArgumentException ae)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = ae.Message,
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Content = ex.Message,
                    CloseButtonText = "Закрыть",
                };
                contentDialog.ShowAsync();
            }
        }

        protected override void Resume() // под вопросом, так как отдельной кнопки продолжить нет (реализуется всё через кнопку start)
        {
            Debug.WriteLine("Play resume TODO");
        }

        protected override void Pause()
        {
            this._recordProcess.Pause();
        }

        protected override void Stop()
        {
            this._recordProcess.Stop();
        }
    }
}

