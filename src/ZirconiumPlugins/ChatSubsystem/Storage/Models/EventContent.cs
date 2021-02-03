using MongoDB.Bson.Serialization.Attributes;

namespace ChatSubsystem.Storage.Models
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(GeneralMessageEventContent))]
    public abstract class EventContent {}

    public class GeneralMessageEventContent : EventContent
    {
        public string Text { get; set; }
        public Media[] Media { get; set; }
        public Reaction[] Reactions { get; set; }
    }
}