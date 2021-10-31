using Newtonsoft.Json;

namespace TrueSkills.APIs
{
    public class ErrorAPI
    {
        [JsonRequired]
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("error")]
        [JsonRequired]
        public string Error { get; set; }
    }
}
