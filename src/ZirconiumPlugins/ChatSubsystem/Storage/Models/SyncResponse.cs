using System.Collections.Generic;
using Newtonsoft.Json;
using Zirconium.Core.Models;

namespace ChatSubsystem.Storage.Models
{
    public class SyncResponse
    {
        [JsonProperty("nextBatch")]
        public EntityID NextBatch { get; set; }
        
        [JsonProperty("mentionedChats")]
        public IList<Chat> MentionedChats { get; set; }
        
        [JsonProperty("events")]
        public IList<Event> Events { get; set; }
    }
}