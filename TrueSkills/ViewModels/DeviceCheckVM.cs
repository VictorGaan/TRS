using ReactiveUI;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using TrueSkills.APIs;
using TrueSkills.Enums;
using TrueSkills.Models;
using TrueSkills.Views;


namespace TrueSkills.ViewModels
{

    public class DeviceCheckVM : ReactiveObject
    {
        private DeviceCheckModel _deviceCheckModel;
        public DeviceCheckModel DeviceCheckModel
        {
            get => _deviceCheckModel;
            set => this.RaiseAndSetIfChanged(ref _deviceCheckModel, value);
        }
        public ReactiveCommand<Unit, Unit> ProceedCommand { get; }
        public ReactiveCommand<Unit, Unit> StartVideoEvent { get; }
        public ReactiveCommand<Unit, Unit> StartPlaybackEvent { get; }
        public ReactiveCommand<Unit, Unit> AudioCommand { get; }
        public ReactiveCommand<Unit, Unit> ValueChangedCommand { get; }

        public DeviceCheckVM()
        {
            _deviceCheckModel = new DeviceCheckModel();
            ProceedCommand = ReactiveCommand.CreateFromTask(Navigate);
            StartVideoEvent = ReactiveCommand.Create(DeviceCheckModel.StartVideo);
            StartPlaybackEvent = ReactiveCommand.Create(DeviceCheckModel.ChangeOutput);
            AudioCommand = ReactiveCommand.Create(DeviceCheckModel.AudioChangeOutput);
            ValueChangedCommand = ReactiveCommand.Create(DeviceCheckModel.ChangeVolume);
        }


        private async Task Navigate()
        {
            var response = await TemporaryVariables.GetStep();
            if (DeviceCheckModel.Documents.Files.Any())
            {
                if (response.Step == Step.ExamHasStartedDocumentDisplayed)
                {
                    TemporaryVariables.s_frame.Navigate(new DocumentsPage());
                }
                if (response.Step == Step.ExamHasStartedModuleNotStarted)
                {
                    BeforeExamWindow beforeExamWindow = new BeforeExamWindow(response);
                    beforeExamWindow.ShowDialog();
                }
            }
            else
            {
                if (response.Step == Step.ExamStartModuleUnderway)
                {
                    TemporaryVariables.s_frame.Navigate(new VMPage());
                }
                if (response.Step == Step.ExamStartTaskDisplay)
                {
                    TemporaryVariables.s_frame.Navigate(new TaskPage());
                }
                if (response.Step == Step.ExamHasStartedModuleNotStarted)
                {
                    BeforeExamWindow beforeExamWindow = new BeforeExamWindow(response);
                    beforeExamWindow.ShowDialog();
                }
            }
            TemporaryVariables.IsAuthDevice = true;
        }
    }

}
