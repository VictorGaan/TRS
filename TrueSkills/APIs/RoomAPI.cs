using Newtonsoft.Json;

namespace TrueSkills.APIs
{
    public class RoomAPI
    {
        public class Rootobject
        {
            [JsonProperty("rooms")]
            public Rooms Rooms { get; set; }
        }

        public class Rooms
        {
            [JsonProperty("expert")]
            public string Expert { get; set; }
            [JsonProperty("support")]
            public string Support { get; set; }
        }
    }
}
