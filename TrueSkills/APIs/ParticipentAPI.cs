using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueSkills.APIs
{
    public class ParticipentAPI
    {
        [JsonProperty("fio")]
        public string FullName { get; set; }
        [JsonProperty("exam")]
        public string Exam { get; set; }
    }
}
