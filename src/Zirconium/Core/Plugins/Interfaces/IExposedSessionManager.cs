using Zirconium.Core.Models;
using System.Collections.Immutable;

namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IExposedSessionManager
    {
        Session GetSession(string sessionID);
        IImmutableDictionary<string, Session> GetSessions();
        void CloseSession(string connID);
    }
}