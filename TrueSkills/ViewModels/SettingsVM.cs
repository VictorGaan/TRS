using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TrueSkills.Enums;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;
using TrueSkills.Models;
using TrueSkills.Views;
using static TrueSkills.APIs.DocumentAPI;

namespace TrueSkills.ViewModels
{
    public class SettingsVM : ReactiveObject, IAsyncInitialization
    {
        private SpecificationModel _specification;
        public ReactiveCommand<Unit, Unit> ProceedCommand { get; }

        public SpecificationModel Specification
        {
            get => _specification;
            set => this.RaiseAndSetIfChanged(ref _specification, value);
        }
        private List<File> _documents;
        public List<File> Documents
        {
            get => _documents;
            set => this.RaiseAndSetIfChanged(ref _documents, value);
        }
        public Task Initialization { get; set; }

        public SettingsVM()
        {
            Initialization = GetDocumentsAsync();
            _specification = new SpecificationModel();
            ProceedCommand = ReactiveCommand.CreateFromTask(Navigate);
        }

        private async Task GetDocumentsAsync()
        {
            if (App.IsNetwork)
            {
                try
                {
                    Documents = await SupportingMethods.GetWebRequest<List<File>>(Url.s_documentUrl, true);
                }
                catch (CodeException ex)
                {

                    TemporaryVariables.ShowException(ex);
                }
            }
        }
        private async Task Navigate()
        {
            var specification = Specification.ServerSpecificationPc.Min;
            if (specification.Webcam != "0" && specification.Microphone != "0")
            {
                TemporaryVariables.frame.Navigate(new DeviceCheckPage());
                return;
            }

            var response = await TemporaryVariables.GetStep();
            if (Documents.Any())
            {
                if (response.Step == Step.ExamHasStartedDocumentDisplayed)
                {
                    TemporaryVariables.frame.Navigate(new DocumentsPage());
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
                    TemporaryVariables.frame.Navigate(new VMPage());
                }
                if (response.Step == Step.ExamStartTaskDisplay)
                {
                    TemporaryVariables.frame.Navigate(new TaskPage());
                }
                if (response.Step == Step.ExamHasStartedModuleNotStarted)
                {
                    BeforeExamWindow beforeExamWindow = new BeforeExamWindow(response);
                    beforeExamWindow.ShowDialog();
                }
            }
            TemporaryVariables.isAuthDevice = true;
        }
    }
}
