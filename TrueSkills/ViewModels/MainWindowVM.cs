using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TrueSkills.Models;
using TrueSkills.Views;

namespace TrueSkills.ViewModels
{
    public class MainWindowVM : ReactiveObject
    {
        public ReactiveCommand<object, Unit> AuthorizationCommand { get; }
        private ParticipentModel _participent;
        public ParticipentModel ParticipentModel
        {
            get => _participent;
            set => this.RaiseAndSetIfChanged(ref _participent, value);
        }
        public MainWindowVM()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                TemporaryVariables.Language = new CultureInfo(args[1]);
            }
            else
            {
                TemporaryVariables.Language = new CultureInfo("ru-RU");
            }
            _participent = new ParticipentModel();
            var canExecute = this.WhenAnyValue(x => x._participent.FullName, x => x._participent.Exam,
                (fullName, exam) =>
                string.IsNullOrEmpty(_participent.GetValidationError(nameof(_participent.FullName))) &&
                string.IsNullOrEmpty(_participent.GetValidationError(nameof(_participent.Exam))));
            AuthorizationCommand = ReactiveCommand.CreateFromTask<object>(async (sender) => await _participent.LoginAsync(sender), canExecute);
        }
    }
}
