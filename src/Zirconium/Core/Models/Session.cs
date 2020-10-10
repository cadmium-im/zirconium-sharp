using System.Net;

namespace Zirconium.Core.Models
{
    public class Session
    {
        public string LastTokenHash { get; set; }
        public SessionAuthData LastTokenPayload { get; set; }
        public IPAddress ClientAddress { get; set; }
        public ConnectionHandler ConnectionHandler { get; set; }
    }
}