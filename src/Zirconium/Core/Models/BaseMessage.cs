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

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("authToken")]
        public string AuthToken { get; set; }

        [JsonProperty("payload")]
        public IDictionary<string, object> Payload { get; set; }

        public BaseMessage() { }

        public BaseMessage(BaseMessage message, bool reply)
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