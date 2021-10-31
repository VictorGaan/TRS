using Newtonsoft.Json;
using TrueSkills.Enums;

namespace TrueSkills.APIs
{
    public class StepAPI
    {
        [JsonProperty("step")]
        public Step Step { get; set; }
        [JsonProperty("end")]
        public string End { get; set; }
    }
}
