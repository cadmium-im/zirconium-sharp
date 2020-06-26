using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zirconium.Core.Models
{
    public class ProtocolError
    {
        [JsonProperty("errCode")]
        public string ErrCode { get; set; }

        [JsonProperty("errText")]
        public string ErrText { get; set; }

        [JsonProperty("errPayload")]
        public IDictionary<string, object> ErrPayload { get; set; }
    }
}