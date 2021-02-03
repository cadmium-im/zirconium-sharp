using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatSubsystem.Storage.Models
{
    public class Chat
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        public string ChatID { get; set; }
        public string AvatarID { get; set; }
        public string Description { get; set; }
        public string[] Members { get; set; }
        public bool IsPrivateChat { get; set; }
        
        public IList<string> Banned { get; set; }
        public IList<string> PinnedMessages { get; set; } // id of pinned messages
        public IDictionary<string, string> RoleNames { get; set; }
        public IDictionary<string, byte[]> Permissions { get; set; }
        public IList<string> LocalAliases { get; set; }
        public IList<string> RemoveAliases { get; set; }
    }
}