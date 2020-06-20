using Newtonsoft.Json;

namespace Zirconium.Core
{
    public class JWTPayload
    {
        [JsonProperty("entityID")]
        public string EntityID { get; set; }

        [JsonProperty("deviceID")]
        public string DeviceID { get; set; }
    }
}