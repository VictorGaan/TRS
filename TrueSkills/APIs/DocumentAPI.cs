using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrueSkills.APIs
{
    public class DocumentAPI
    {
        public class Rootobject
        {
            [JsonProperty("files")]
            public IEnumerable<File> Files { get; set; }
        }

        public class File
        {
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("id")]
            public string Id { get; set; }
        }

    }
}
