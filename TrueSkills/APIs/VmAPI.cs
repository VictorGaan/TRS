using Newtonsoft.Json;

namespace TrueSkills.APIs
{
    public class VmAPI
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
