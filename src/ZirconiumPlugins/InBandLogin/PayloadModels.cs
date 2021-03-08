using Newtonsoft.Json;

namespace InBandLogin
{
    class LoginRequestPayload
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    class LoginResponsePayload
    {
        [JsonProperty("authToken")]
        public string AuthToken { get; set; }

        [JsonProperty("deviceID")]
        public string DeviceID { get; set; }
    }

    class RegisterRequestPayload
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    class RegisterResponsePayload
    {
        [JsonProperty("userID")]
        public string UserID { get; set; }

        [JsonProperty("authToken")]
        public string AuthToken { get; set; }

        [JsonProperty("deviceID")]
        public string DeviceID { get; set; }
    }
}