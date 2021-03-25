using System.Collections.Generic;
using Zirconium.Core.Models;

namespace ChatSubsystem.Storage.Models
{
    public class SyncResponse
    {
        public EntityID NextBatch { get; set; }
        public IList<Chat> MentionedChats { get; set; }
        public IList<Event> Events { get; set; }
    }
}