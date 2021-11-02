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
    public class DocumentsVM : ReactiveObject
    {
        private DocumentModel _documentModel;
        public ReactiveCommand<Unit, Unit> CheckCommand { get; set; }
        public DocumentModel DocumentModel
        {
            get => _documentModel;
            set => this.RaiseAndSetIfChanged(ref _documentModel, value);
        }
        public DocumentsVM()
        {
            DocumentModel = new DocumentModel();
            CheckCommand = ReactiveCommand.CreateFromTask(FitAndNavigateAsync);
        }
        private async Task FitAndNavigateAsync()
        {
            await DocumentModel.FitDocumentsAsync();
            var response = await TemporaryVariables.GetStep();
            if (DocumentModel.Pdfs.Count == 0)
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
        }
    }
}
