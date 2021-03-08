using System.Collections.Generic;
using Zirconium.Core.Models;
using Zirconium.Core.Models.Authorization;

namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IAuthProvider
    {
        (SessionAuthData, AuthorizationResponse) TestAuthFields(IDictionary<string, dynamic> fields);
        
        EntityID GetEntityID(IDictionary<string, dynamic> fields);

        // User registration logic
        void CreateUser(string username, string pass);
        string GetAuthProviderName();
        string[] GetAuthSupportedMethods();
    }
}