using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessageStorage
{
    public class Message
    {
        public ObjectId Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string RoomId { get; set; }
        public string Type { get; set; }
        public long Timestamp { get; set; }
        public string Content { get; set; }
        public Media[] Media { get; set; }
        public Reaction[] Reactions { get; set; }
        
        [BsonExtraElements]
        public BsonDocument OtherMetadata { get; set; }
    }

    public class Media
    {
        public string MimeType;
        public string Url;
    }

    public class Reaction
    {
        public string ID { get; set; }
    }
}