using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrueSkills.APIs
{
    public class SizeAPI
    {
        [JsonProperty("width")]
        public double Width { get; set; }
        [JsonProperty("height")]
        public double Height { get; set; }
    }
}
