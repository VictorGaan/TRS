using Newtonsoft.Json;
using ReactiveUI;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TrueSkills.Enums;

namespace TrueSkills.APIs
{
    public class ServerSpecificationPcAPI
    {
        [DataContract]
        public class Rootobject : ReactiveObject
        {
            [IgnoreDataMember]
            private Min _min;
            [IgnoreDataMember]
            private Status _status;
            public Rootobject()
            {
                _min = new Min();
                _status = new Status();
            }
            [DataMember]
            [JsonProperty("min")]
            public Min Min
            {
                get => _min;
                set => this.RaiseAndSetIfChanged(ref _min, value);
            }
            [DataMember]
            [JsonProperty("status")]
            public Status Status
            {
                get => _status;
                set => this.RaiseAndSetIfChanged(ref _status, value);
            }
        }

        public class Min : ReactiveObject
        {
            [IgnoreDataMember]
            private string _webcam;
            [IgnoreDataMember]
            private string _microphone;
            [IgnoreDataMember]
            private CPU _cpu;
            [IgnoreDataMember]
            private RAM _ram;
            [IgnoreDataMember]
            private Freespace _freeSpace;
            [IgnoreDataMember]
            private Internet _internet;
            [IgnoreDataMember]
            private List<Monitor> _monitors;
            public Min()
            {
                _webcam = string.Empty;
                _microphone = string.Empty;
                _cpu = new CPU();
                _ram = new RAM();
                _freeSpace = new Freespace();
                _internet = new Internet();
                _monitors = new List<Monitor>();
            }
            [DataMember]
            public CPU CPU
            {
                get => _cpu;
                set => this.RaiseAndSetIfChanged(ref _cpu, value);
            }
            [DataMember]
            public RAM RAM
            {
                get => _ram;
                set => this.RaiseAndSetIfChanged(ref _ram, value);
            }
            [DataMember]
            public Internet Internet
            {
                get => _internet;
                set => this.RaiseAndSetIfChanged(ref _internet, value);
            }
            [DataMember]
            public Freespace FreeSpace
            {
                get => _freeSpace;
                set => this.RaiseAndSetIfChanged(ref _freeSpace, value);
            }
            [DataMember]
            public List<Monitor> Monitors
            {
                get => _monitors;
                set => this.RaiseAndSetIfChanged(ref _monitors, value);
            }
            [DataMember]
            public string Webcam
            {
                get => _webcam;
                set => this.RaiseAndSetIfChanged(ref _webcam, value);
            }
            [DataMember]
            public string Microphone
            {
                get => _microphone;
                set => this.RaiseAndSetIfChanged(ref _microphone, value);
            }
        }

        public class CPU : ReactiveObject
        {
            [IgnoreDataMember]
            private string _name;
            public CPU()
            {
                _name = string.Empty;
            }
            [DataMember]
            [JsonProperty("name")]
            public string Name
            {
                get => _name;
                set => this.RaiseAndSetIfChanged(ref _name, value);
            }
        }

        public class RAM : ReactiveObject
        {
            [IgnoreDataMember]
            private int _value;
            [IgnoreDataMember]
            private string _measurement;
            public RAM()
            {
                _value = 0;
                _measurement = string.Empty;
            }
            [DataMember]
            [JsonProperty("value")]
            public int Value
            {
                get => _value;
                set => this.RaiseAndSetIfChanged(ref _value, value);
            }
            [DataMember]
            [JsonProperty("measurement")]
            public string Measurement
            {
                get => _measurement;
                set => this.RaiseAndSetIfChanged(ref _measurement, value);
            }
        }

        public class Internet : ReactiveObject
        {
            [IgnoreDataMember]
            private long _value;
            [IgnoreDataMember]
            private string _measurement;
            public Internet()
            {
                _value = 0;
                _measurement = string.Empty;
            }
            [DataMember]
            [JsonProperty("value")]
            public long Value
            {
                get => _value;
                set => this.RaiseAndSetIfChanged(ref _value, value);
            }
            [DataMember]
            [JsonProperty("measurement")]
            public string Measurement
            {
                get => _measurement;
                set => this.RaiseAndSetIfChanged(ref _measurement, value);
            }
        }

        public class Freespace : ReactiveObject
        {
            [IgnoreDataMember]
            private long _value;
            [IgnoreDataMember]
            private string _measurement;
            public Freespace()
            {
                _value = 0;
                _measurement = string.Empty;
            }
            [DataMember]
            [JsonProperty("value")]
            public long Value
            {
                get => _value;
                set => this.RaiseAndSetIfChanged(ref _value, value);
            }
            [DataMember]
            [JsonProperty("measurement")]
            public string Measurement
            {
                get => _measurement;
                set => this.RaiseAndSetIfChanged(ref _measurement, value);
            }
        }

        public class Monitor : ReactiveObject
        {
            [IgnoreDataMember]
            private int _value;
            [IgnoreDataMember]
            private string _measurement;
            public Monitor()
            {
                _value = 0;
                _measurement = string.Empty;
            }
            [DataMember]
            [JsonProperty("value")]
            public int Value
            {
                get => _value;
                set => this.RaiseAndSetIfChanged(ref _value, value);
            }
            [DataMember]
            [JsonProperty("measurement")]
            public string Measurement
            {
                get => _measurement;
                set => this.RaiseAndSetIfChanged(ref _measurement, value);
            }
        }

        public class Status
        {
            [DataMember]
            public StatusSpecification Webcam { get; set; }
            [DataMember]
            public StatusSpecification Microphone { get; set; }
            [DataMember]
            public StatusSpecification RAM { get; set; }
            [DataMember]
            public StatusSpecification Internet { get; set; }
            [DataMember]
            public StatusSpecification FreeSpace { get; set; }
            [DataMember]
            public StatusMonitor[] Monitors { get; set; }
            [DataMember]
            public StatusSpecification CPU { get; set; }
        }

        public class StatusMonitor : ReactiveObject
        {
            [IgnoreDataMember]
            private int _value;
            [IgnoreDataMember]
            private StatusSpecification _measurement;
            public StatusMonitor()
            {
                _value = 0;
                _measurement = 0;
            }
            [DataMember]
            [JsonProperty("value")]
            public int Value
            {
                get => _value;
                set => this.RaiseAndSetIfChanged(ref _value, value);
            }
            [DataMember]
            [JsonProperty("status")]
            public StatusSpecification Measurement
            {
                get => _measurement;
                set => this.RaiseAndSetIfChanged(ref _measurement, value);
            }
        }
    }
}
