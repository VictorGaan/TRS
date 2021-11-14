using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrueSkills.Enums;
using TrueSkills.Interfaces;
using TrueSkills.Models;

namespace TrueSkills.ViewModels
{
    public class ChatVM : ReactiveObject, IAsyncInitialization
    {
        private ChatModel _chatModel;

        public ReactiveCommand<Unit, Unit> SendCommand { get; }
        public ReactiveCommand<KeyEventArgs, Unit> KeyDownEvent { get; }
        public ChatModel ChatModel
        {
            get => _chatModel;
            set => this.RaiseAndSetIfChanged(ref _chatModel, value);
        }
        public Task Initialization { get; set; }
        public ChatVM(Room room)
        {
            _chatModel = new ChatModel();
            Initialization = ChatModel.GetMessagesAsync(room);
            var canExecute = this.WhenAnyValue(x => x._chatModel.Message,
                (message) =>
                !string.IsNullOrWhiteSpace(message));
            SendCommand = ReactiveCommand.CreateFromTask(async Task => await _chatModel.SendMessageAsync(room), canExecute);
            KeyDownEvent = ReactiveCommand.CreateFromTask<KeyEventArgs>(async e => await _chatModel.SendMessageAsync(room, e.Key), canExecute);
        }
    }
}
