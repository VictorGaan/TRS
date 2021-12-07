using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrueSkills.APIs
{
    public class DocumentAPI
    {
        public class File
        {

            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("id")]
            public string Id { get; set; }
        }

    }
}
