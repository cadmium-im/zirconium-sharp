using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zirconium.Core.Models
{
    public class BaseMessage
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("type")]
        public string MessageType { get; set; }

        [JsonProperty("from", NullValueHandling = NullValueHandling.Ignore)]
        public string From { get; set; }

        [JsonProperty("to", NullValueHandling = NullValueHandling.Ignore)]
        public string To { get; set; }

        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("authToken", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthToken { get; set; }

        [JsonProperty("payload")]
        public IDictionary<string, object> Payload { get; set; }

        public BaseMessage() { }

        public BaseMessage(BaseMessage message, bool reply)
        {
            Payload = new Dictionary<string, object>();
            if (message != null)
            {
                ID = message.ID;
                MessageType = message.MessageType;
                if (reply)
                {
                    From = message.To;
                    To = message.From;
                }
                else
                {
                    From = message.From;
                    To = message.To;
                }

                Ok = message.Ok;
                AuthToken = message.AuthToken;
                Payload = message.Payload;
            }
        }
    }
}