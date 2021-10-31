using ReactiveUI;
using System.Linq;
using System.Reactive;
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

        public DeviceCheckVM()
        {
            _deviceCheckModel = new DeviceCheckModel();
            ProceedCommand = ReactiveCommand.Create(Navigate);
            StartVideoEvent = ReactiveCommand.Create(DeviceCheckModel.StartVideo);
            StartPlaybackEvent = ReactiveCommand.Create(DeviceCheckModel.ChangeOutput);
            AudioCommand = ReactiveCommand.Create(DeviceCheckModel.AudioChangeOutput);
        }


        private void Navigate()
        {
            if (DeviceCheckModel.Documents.Files.Any())
            {
                if (TemporaryVariables.s_step == Step.ExamHasStartedDocumentDisplayed)
                {
                    TemporaryVariables.s_frame.Navigate(new DocumentsPage(DeviceCheckModel.Documents));
                }
            }
            else
            {
                if (TemporaryVariables.s_step == Step.ExamStartModuleUnderway)
                {
                    TemporaryVariables.s_frame.Navigate(new VMPage());
                }
                else
                {
                    if (TemporaryVariables.s_step == Step.ExamStartTaskDisplay)
                    {
                        TemporaryVariables.s_frame.Navigate(new TaskPage());
                    }
                    else
                    {
                        BeforeExamWindow beforeExamWindow = new BeforeExamWindow();
                        beforeExamWindow.ShowDialog();
                    }
                }
            }
        }

    }
}
