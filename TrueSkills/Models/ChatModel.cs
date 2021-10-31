﻿using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TrueSkills.Enums;
using TrueSkills.Exceptions;
using TrueSkills.Interfaces;
using static TrueSkills.APIs.MessageAPI;

namespace TrueSkills.Models
{
    public class ChatModel : ReactiveObject, IAsyncInitialization
    {
        public Task Initialization { get; set; }
        private string _message;
        private ObservableCollection<Message> _messages;
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }
        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set => this.RaiseAndSetIfChanged(ref _messages, value);
        }

        public ChatModel()
        {

        }

        public async Task GetMessagesAsync(Room room)
        {
            var url = await TemporaryVariables.GetUrlAsync(room, Operation.Get);
            try
            {
                var response = await SupportingMethods.PostWebRequest<Rootobject>(url, true);
                Messages = response.Messages;
                await Task.Delay(5000);
                await GetMessagesAsync(room);
            }
            catch (CodeException ex)
            {
                TemporaryVariables.ShowException(ex);
            }
        }

        public async Task SendMessageAsync(Room room)
        {
            var url = await TemporaryVariables.GetUrlAsync(room, Operation.Send);
            var request = new { text = Message };
            try
            {
                await SupportingMethods.PostWebRequest(url, request, true);
            }
            catch (CodeException ex)
            {
                TemporaryVariables.ShowException(ex);
            }
            Message = string.Empty;
        }
    }
}
