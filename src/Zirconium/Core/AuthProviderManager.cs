using System.Collections.Generic;
using System;
using Zirconium.Core.Plugins.Interfaces;
using System.Linq;

namespace Zirconium.Core
{
    public class AuthProviderManager
    {
        private App _app;
        private IList<IAuthProvider> _authProviders;
        public IAuthProvider DefaultAuthProvider { get; private set; }

        public AuthProviderManager(App app)
        {
            _app = app;
            _authProviders = new List<IAuthProvider>();
            DefaultAuthProvider = null;
        }

        public void AddAuthProvider(IAuthProvider provider)
        {
            _authProviders.Add(provider);
        }

        public void SetDefaultAuthProvider() {
            DefaultAuthProvider = _authProviders.FirstOrDefault(x => x.GetAuthProviderName() == _app.Config.AuthenticationProvider);
        }
    }
}