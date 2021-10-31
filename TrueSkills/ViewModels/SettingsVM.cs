using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TrueSkills.Enums;
using TrueSkills.Models;
using TrueSkills.Views;

namespace TrueSkills.ViewModels
{
    public class SettingsVM : ReactiveObject
    {
        private SpecificationModel _specification;
        public ReactiveCommand<Unit, Unit> ProceedCommand { get; }

        public SpecificationModel Specification
        {
            get => _specification;
            set => this.RaiseAndSetIfChanged(ref _specification, value);
        }

        public SettingsVM()
        {
            _specification = new SpecificationModel();
            ProceedCommand = ReactiveCommand.Create(Navigate);
        }
        private void Navigate()
        {
            TemporaryVariables.s_frame.Navigate(new DeviceCheckPage());
        }
    }
}
