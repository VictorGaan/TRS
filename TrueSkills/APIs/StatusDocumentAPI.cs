using Newtonsoft.Json;

namespace TrueSkills.APIs
{
    public class StatusDocumentAPI
    {
        [JsonProperty("statusEdit")]
        public bool IsStatusEdit { get; set; }
    }
}
