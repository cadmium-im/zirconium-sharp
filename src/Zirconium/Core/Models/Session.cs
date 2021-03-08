using System.Net;

namespace Zirconium.Core.Models
{
    public class Session
    {
        public SessionAuthData AuthData { get; set; }
        public IPAddress ClientAddress { get; set; }
        public ConnectionHandler ConnectionHandler { get; set; }
    }
}