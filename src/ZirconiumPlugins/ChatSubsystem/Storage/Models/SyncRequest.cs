using Zirconium.Core.Models;

namespace ChatSubsystem.Storage.Models
{
    public class SyncRequest
    {
        public EntityID Since { get; set; }
        public int Limit { get; set; }
        public EntityID ChatID { get; set; }
        public string SyncDirection { get; set; }
    }
}