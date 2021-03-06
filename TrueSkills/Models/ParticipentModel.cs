using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using TrueSkills.APIs;
using TrueSkills.Enums;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;
using TrueSkills.Views;

namespace TrueSkills.Models
{
    public class ParticipentModel : ReactiveObject, IDataErrorInfo
    {
        private string _token;
        private string _fullName;
        private string _exam;
        public string Token
        {
            get => _token;
            set => this.RaiseAndSetIfChanged(ref _token, value);
        }
        public string FullName
        {
            get => _fullName;
            set => this.RaiseAndSetIfChanged(ref _fullName, value);
        }
        public string Exam
        {
            get => _exam;
            set => this.RaiseAndSetIfChanged(ref _exam, value);
        }

        public ParticipentModel()
        {
            _token = string.Empty;
            _fullName = string.Empty;
            _exam = string.Empty;
        }


        public async Task LoginAsync(object sender)
        {
            if (App.IsNetwork)
            {
                var serializeObject = new ParticipentAPI() { FullName = FullName, Exam = Exam };
                try
                {
                    var response = await SupportingMethods.PostWebRequest<ParticipentAPI, TokenAPI>(Url.s_authUrl, serializeObject);
                    _token = response.Token;
                    TemporaryVariables.currentParticipent = this;
                    await TemporaryVariables.GetStream();
                    var step = await TemporaryVariables.GetStep();
                    if (step.Step == Step.ExamNotRun || step.Step == Step.ExamHasStartedModuleNotStarted)
                    {
                        BeforeExamWindow beforeExamWindow = new BeforeExamWindow(step);
                        beforeExamWindow.Show();
                        CloseThisWindow(sender);
                        return;
                    }
                    if (step.Step == Step.ExamStartTaskDisplay || step.Step == Step.ExamHasStartedDocumentDisplayed
                        || step.Step == Step.ExamStartModuleUnderway)
                    {
                        CloseOpenWindow(sender);
                        return;
                    }
                    if (step.Step == Step.ExamOver)
                    {
                        CloseWindow(sender);
                    }
                }
                catch (CodeException ex)
                {
                    TemporaryVariables.ShowException(ex);
                }
            }
            else
            {
                TemporaryVariables.NotNetwork();
            }
        }

        public string Error => throw new NotImplementedException();


        public string this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

        public string GetValidationError(string propertyName)
        {
            string error = null;
            switch (propertyName)
            {
                case "FullName":
                    if (string.IsNullOrWhiteSpace(FullName))
                    {
                        error = $"{TemporaryVariables.GetProperty("tm_EmptyError")}";
                        break;
                    }
                    if (FullName.Length > 0)
                    {
                        if (FullName.Where(x => char.IsDigit(x)).Any())
                        {
                            error = $"{TemporaryVariables.GetProperty("tm_NumberError")}";
                        }

                        if (FullName.Where(x => !IsBasicLetterFn(x) && !IsBasicLetterFnRus(x)).Any())
                        {
                            error = $"{TemporaryVariables.GetProperty("tm_PunctuationError")}";
                        }
                    }

                    if ((FullName.Length < 5) || (FullName.Length > 255))
                    {

                        error = $"{TemporaryVariables.GetProperty("tm_RangeError")}";
                    }
                    break;
                case "Exam":
                    if (string.IsNullOrWhiteSpace(Exam))
                    {
                        error = $"{TemporaryVariables.GetProperty("tm_EmptyError")}";
                        break;
                    }

                    if (Exam.Length > 0)
                    {
                        if (Exam.Where(x => !IsBasicLetter(x) && !char.IsDigit(x)).Any())
                        {
                            error = $"{TemporaryVariables.GetProperty("tm_LatinOrNumberError")}";
                        }
                    }


                    if (Exam.IndexOf(' ') != -1)
                    {
                        error = $"{TemporaryVariables.GetProperty("tm_WhiteSpaceError")}";
                    }


                    if ((Exam.Length < 2) || (Exam.Length > 8))
                    {
                        error = $"{TemporaryVariables.GetProperty("tm_RangeExamError")}";
                    }
                    break;
            }

            return error;
        }

        private bool IsBasicLetterFnRus(char c)
        {
            return (c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я') || c == ' ' || c == '-';
        }

        private bool IsBasicLetterFn(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == ' ' || c == '-';
        }

        private bool IsBasicLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }
        private void CloseThisWindow(object sender)
        {
            (sender as MainWindow).Close();
        }

        private void CloseWindow(object sender)
        {
            new ExamEndWindow().Show();
            (sender as MainWindow).Close();
        }

        private void CloseOpenWindow(object sender)
        {
            new NavigationWindow().Show();
            (sender as MainWindow).Close();
        }
    }
}
