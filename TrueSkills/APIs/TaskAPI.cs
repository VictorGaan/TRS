using Newtonsoft.Json;

namespace TrueSkills.APIs
{
    public class TaskAPI
    {
        public class Rootobject
        {
            [JsonProperty("files")]
            public File[] Files { get; set; }
        }
        public class File
        {
            [JsonProperty("url")]
            public string Url { get; set; }
        }
    }
}
