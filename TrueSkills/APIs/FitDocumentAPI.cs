using Newtonsoft.Json;

namespace TrueSkills.APIs
{
    public class FitDocumentAPI
    {
        [JsonProperty("status")]
        public bool IsStatus { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
