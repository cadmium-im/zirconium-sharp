namespace Zirconium.Core.Models
{
    public class EntityID
    {
        public string LocalPart { get; }
        public string ServerPart { get; }
        public bool OnlyServerPart { get; }
        public char Type { get; }

        public EntityID(string entityID)
        {
            Type = entityID[0];
            entityID = entityID.Remove(0, 1); // prevent confusion with username entityID type when splitting
            var entityIdSplitted = entityID.Split("@");
            
            // if it is only server part
            if (entityIdSplitted.Length == 1 && entityIdSplitted[0] == entityID)
            {
                ServerPart = entityIdSplitted[0];
                OnlyServerPart = true;
            } 
            
            LocalPart = entityIdSplitted[0];
            ServerPart = entityIdSplitted[1];
        }

        public EntityID(char type, string localPart, string serverPart)
        {
            Type = type;
            LocalPart = localPart;
            ServerPart = serverPart;
        }

        public override string ToString()
        {
            if (OnlyServerPart)
            {
                return ServerPart;
            } 
            return $"{Type}{LocalPart}@{ServerPart}";
        }
    }
}