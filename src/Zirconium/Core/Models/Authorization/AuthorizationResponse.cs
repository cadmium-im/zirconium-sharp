using Newtonsoft.Json;

namespace Zirconium.Core.Models.Authorization
{
    public class AuthorizationResponse
    {
        [JsonProperty("token")]
        public string Token;
        
        [JsonProperty("deviceID")]
        public string DeviceID { get; set; }
    }
}