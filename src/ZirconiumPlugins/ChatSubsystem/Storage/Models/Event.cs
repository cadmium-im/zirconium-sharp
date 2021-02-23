using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Zirconium.Core.Models;

namespace ChatSubsystem.Storage.Models
{
    public class Event
    {
        [BsonId] public ObjectId Id { get; set; }

        public EntityID EventID { get; set; }
        public EntityID From { get; set; }
        public EntityID ChatId { get; set; }
        public string Type { get; set; }
        public long Timestamp { get; set; }
        public EntityID[] PrevEvents { get; set; }
        public ObjectId PrevID { get; set; }
        public EntityID OriginServer { get; set; }
        public EventContent Content { get; set; }
    }

    public class EventWithChildren : Event
    {
        public Event[] Children { get; set; }
    }
}