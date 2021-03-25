using Newtonsoft.Json;
using Zirconium.Core.Models;

namespace ChatSubsystem.Storage.Models
{
    public class SyncRequest
    {
        [JsonProperty("since")]
        public EntityID Since { get; set; }
        
        [JsonProperty("limit")]
        public int Limit { get; set; }
        
        [JsonProperty("chatID")]
        public EntityID ChatID { get; set; }
        
        [JsonProperty("syncDirection")]
        public string SyncDirection { get; set; }
    }
}