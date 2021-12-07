using Newtonsoft.Json;

namespace TrueSkills.APIs
{
    public class TaskAPI
    {
        public class File
        {
            [JsonProperty("url")]
            public string Url { get; set; }
        }
    }
}
