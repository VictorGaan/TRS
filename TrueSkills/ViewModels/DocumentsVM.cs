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
        public DocumentsVM(DocumentAPI.Rootobject rootObject)
        {
            DocumentModel = new DocumentModel(rootObject);
            CheckCommand = ReactiveCommand.CreateFromTask(FitAndNavigateAsync);
        }
        private async Task FitAndNavigateAsync()
        {
            await DocumentModel.FitDocumentsAsync();
            if (DocumentModel.Pdfs.Count == 0)
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
