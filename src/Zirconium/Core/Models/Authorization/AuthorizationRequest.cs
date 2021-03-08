using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zirconium.Core.Models.Authorization
{
    public class AuthorizationRequest
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("fields")]
        public IDictionary<string, dynamic> Fields { get; set; }
    }
}