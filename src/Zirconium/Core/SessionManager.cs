using System.Collections.Immutable;
using System.Collections.Generic;
using Zirconium.Core.Models;
using Zirconium.Core.Plugins.Interfaces;

namespace Zirconium.Core
{
    public class SessionManager: IExposedSessionManager
    {
        private IDictionary<string, Session> _sessions;

        public SessionManager()
        {
            _sessions = new Dictionary<string, Session>();
        }

        public void AddSession(string connID, Session session) {
            _sessions[connID] = session;
        }

        // Get session using specified ID
        // <exception cref="KeyNotFoundException">Throws this exception when connection is not found</exception>
        public Session GetSession(string sessionID) {
            return _sessions[sessionID];
        }

        public void DeleteSession(string connID) {
            _sessions.Remove(connID);
        }

        public IImmutableDictionary<string, Session> GetSessions() {
            return _sessions.ToImmutableDictionary();
        }

        public void CloseSession(string connID) {
            _sessions[connID].ConnectionHandler.CloseConnection();
        }
    }
}