using System.Net;

namespace Zirconium.Core.Models
{
    public class ConnectionInfo
    {
        public string LastTokenHash { get; set; }
        public JWTPayload LastTokenPayload { get; set; }
        public IPAddress ClientAddress { get; set; }
    }
}