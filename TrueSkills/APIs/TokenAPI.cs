using Newtonsoft.Json;

namespace TrueSkills.APIs
{
    public class TokenAPI
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
