using System.Collections.Generic;
using Zirconium.Core.Models;

namespace Zirconium.Core
{
    public class SessionManager
    {
        private IDictionary<string, ConnectionInfo> _sessions;

        public SessionManager()
        {
            _sessions = new Dictionary<string, ConnectionInfo>();
        }

        public void AddSession(string connID, ConnectionInfo connInfo) {
            _sessions[connID] = connInfo;
        }

        // Get connection info about specified connection ID
        // <exception cref="KeyNotFoundException">Throws this exception when connection is not found</exception>
        public ConnectionInfo GetConnectionInfo(string connID) {
            return _sessions[connID];
        }

        public void DeleteSession(string connID) {
            _sessions.Remove(connID);
        }
    }
}