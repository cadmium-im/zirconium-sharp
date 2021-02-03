namespace ChatSubsystem.Storage.Models
{
    public class Media
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public int Size { get; set; }
        public string FileName { get; set; }
        public MediaAttrs Attrs { get; set; }
    }
}