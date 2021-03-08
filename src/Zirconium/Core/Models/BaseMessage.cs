using System;
using System.Linq;
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
        public string[] To { get; set; }

        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("payload", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> Payload { get; set; }

        public BaseMessage() { 
            Payload = new Dictionary<string, object>();
            ID = Guid.NewGuid().ToString();
        }

        public BaseMessage(string type) : this()
        {
            MessageType = type;
        }

        public BaseMessage(BaseMessage message, bool reply) : this()
        {
            if (message != null)
            {
                ID = message.ID;
                MessageType = message.MessageType;
                if (reply)
                {
                    // TODO probably need to fix it
                    From = message.To.First();
                    To = new[] { message.From };
                }
                else
                {
                    From = message.From;
                    To = message.To;
                }

                Ok = message.Ok;
                Payload = message.Payload;
            }
        }
    }
}