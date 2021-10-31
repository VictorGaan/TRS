using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrueSkills.APIs
{
    public class NowAPI
    {
        [JsonProperty("time")]
        public string Time { get; set; }
    }
}
