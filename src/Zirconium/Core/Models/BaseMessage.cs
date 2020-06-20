using System.Collections.Generic;

namespace Zirconium.Core.Models
{
    public class BaseMessage
    {
        public string ID {get; set;}
        public string MessageType {get; set;}
        public string From {get; set;}
        public string To {get; set;}
        public bool Ok {get; set;}
        public string AuthToken {get; set;}
        public IDictionary<string, object> Payload {get; set;}
    }
}