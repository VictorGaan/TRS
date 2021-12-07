using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace TrueSkills.APIs
{
    public class MessageAPI
    {
        public class Rootobject : ReactiveObject
        {
            private ObservableCollection<Message> _messages;
            public ObservableCollection<Message> Messages
            {
                get => _messages;
                set => this.RaiseAndSetIfChanged(ref _messages, value);
            }
        }

        public class Message : ReactiveObject
        {
            private string _fullName;
            private string _messageId;
            private string _text;
            private string _date;
            [JsonProperty("user")]
            public string FullNameUser
            {
                get => _fullName;
                set => this.RaiseAndSetIfChanged(ref _fullName, value);
            }
            [JsonProperty("message")]
            public string MessageId
            {
                get => _messageId;
                set => this.RaiseAndSetIfChanged(ref _messageId, value);
            }
            [JsonProperty("text")]
            public string Text
            {
                get => _text;
                set => this.RaiseAndSetIfChanged(ref _text, value);
            }
            [JsonProperty("date")]
            public string Date
            {
                get => _date;
                set => this.RaiseAndSetIfChanged(ref _date, value);
            }
            [JsonIgnore]
            public DateTime? DateMessage
            {
                get
                {
                    if (Date != null)
                    {
                        if (DateTime.TryParse(Date, out DateTime dateTime))
                        {
                            return dateTime;
                        }
                    }
                    return null;
                }
            }
            [JsonIgnore]
            public bool IsOwner
            {
                get
                {
                    if (TemporaryVariables.currentParticipent == null)
                        return false;
                    if (FullNameUser == TemporaryVariables.currentParticipent.FullName)
                        return true;
                    return false;
                }
            }
        }

    }
}
