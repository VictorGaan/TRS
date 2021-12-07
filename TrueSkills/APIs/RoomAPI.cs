using Newtonsoft.Json;

namespace TrueSkills.APIs
{
    public class RoomAPI
    {
        public class Rooms
        {
            [JsonProperty("expert")]
            public string Expert { get; set; }
            [JsonProperty("support")]
            public string Support { get; set; }
        }
    }
}
