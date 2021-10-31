using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TrueSkills.APIs
{
    public class SpecificationPcAPI
    {
        [DataContract]
        public class Rootobject : ReactiveObject
        {
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

            public Rootobject()
            {
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
        }

        public class CPU : ReactiveObject
        {
            private string _name;
            public CPU()
            {
                _name = string.Empty;
            }

            [JsonProperty("name")]
            public string Name
            {
                get => _name;
                set => this.RaiseAndSetIfChanged(ref _name, value);
            }
        }

        public class RAM : ReactiveObject
        {
            private int _value;
            private string _measurement;
            public RAM()
            {
                _value = 0;
                _measurement = string.Empty;
            }
            [JsonProperty("value")]
            public int Value
            {
                get => _value;
                set => this.RaiseAndSetIfChanged(ref _value, value);
            }
            [JsonProperty("measurement")]
            public string Measurement
            {
                get => _measurement;
                set => this.RaiseAndSetIfChanged(ref _measurement, value);
            }
        }

        public class Internet : ReactiveObject
        {
            private long _value;
            private string _measurement;
            public Internet()
            {
                _value = 0;
                _measurement = string.Empty;
            }
            [JsonProperty("value")]
            public long Value
            {
                get => _value;
                set => this.RaiseAndSetIfChanged(ref _value, value);
            }
            [JsonProperty("measurement")]
            public string Measurement
            {
                get => _measurement;
                set => this.RaiseAndSetIfChanged(ref _measurement, value);
            }
        }

        public class Freespace : ReactiveObject
        {
            private long _value;
            private string _measurement;
            public Freespace()
            {
                _value = 0;
                _measurement = string.Empty;
            }
            [JsonProperty("value")]
            public long Value
            {
                get => _value;
                set => this.RaiseAndSetIfChanged(ref _value, value);
            }
            [JsonProperty("measurement")]
            public string Measurement
            {
                get => _measurement;
                set => this.RaiseAndSetIfChanged(ref _measurement, value);
            }
        }

        public class Monitor : ReactiveObject
        {
            private int _value;
            private string _measurement;
            public Monitor()
            {
                _value = 0;
                _measurement = string.Empty;
            }
            [JsonProperty("value")]
            public int Value
            {
                get => _value;
                set => this.RaiseAndSetIfChanged(ref _value, value);
            }
            [JsonProperty("measurement")]
            public string Measurement
            {
                get => _measurement;
                set => this.RaiseAndSetIfChanged(ref _measurement, value);
            }
        }
    }
}
