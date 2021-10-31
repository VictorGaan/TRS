using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrueSkills.APIs
{
    public class StreamAPI
    {
        [JsonProperty("camera")]
        public string Camera { get; set; }
        [JsonProperty("screen")]
        public string Screen { get; set; }
    }
}
