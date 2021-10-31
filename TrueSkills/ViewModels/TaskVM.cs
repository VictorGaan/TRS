using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TrueSkills.Models;

namespace TrueSkills.ViewModels
{
    public class TaskVM : ReactiveObject
    {
        private TaskModel _taskModel;
        public ReactiveCommand<Unit, Unit> NavCommandDown { get; set; }
        public ReactiveCommand<Unit, Unit> NavCommandUp { get; set; }
       
        public TaskModel TaskModel
        {
            get => _taskModel;
            set => this.RaiseAndSetIfChanged(ref _taskModel, value);
        }
        public TaskVM()
        {
            _taskModel = new TaskModel();
            NavCommandDown = ReactiveCommand.Create(() => TaskModel.NextDocument(isUp: false));
            NavCommandUp = ReactiveCommand.Create(() => TaskModel.NextDocument());
        }
    }
}
