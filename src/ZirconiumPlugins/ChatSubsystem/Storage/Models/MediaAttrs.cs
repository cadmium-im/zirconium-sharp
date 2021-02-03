using MongoDB.Bson.Serialization.Attributes;

namespace ChatSubsystem.Storage.Models
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(AudioAttrs),typeof(StickerAttrs), typeof(VideoAttrs), typeof(PhotoAttrs), typeof(GeolocationAttrs))]
    public abstract class MediaAttrs
    {
    }
    
    public class AudioAttrs : MediaAttrs
    {
        public bool Voice { get; set; } // whether it is voice message
        public decimal Duration { get; set; } // duration of audio
        public string? Title { get; set; } // audio title
        public string? Artist { get; set; } // audio artist
        public byte[] VoiceWaveForm { get; set; } // voice wave form
    }

    public class StickerAttrs : MediaAttrs
    {
        public string AssociatedEmoji { get; set; } // the associated emoji with this
        public string StickerSetID { get; set; } // id of sticker set which is associated with this sticker
        public bool Animated { get; set; } // is client need to animate this sticker
    }

    public class VideoAttrs : MediaAttrs
    {
        public bool IsVideoMessage { get; set; } // is this a rounded video message
        public decimal Duration { get; set; } // duration of video
        public decimal Width { get; set; } // width of video
        public decimal Height { get; set; } // height of video
    }

    public class PhotoAttrs : MediaAttrs
    {
        public decimal Width { get; set; } // width of photo
        public decimal Height { get; set; } // height of photo
    }

    public class GeolocationAttrs : MediaAttrs
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}