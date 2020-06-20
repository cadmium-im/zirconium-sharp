using System;
using Zirconium.Core.Models;
using Zirconium.Core.Modules.Interfaces;

namespace Zirconium.Core.Modules
{
    public class HostModuleAPI : IHostModuleAPI
    {
        private Router _router;

        public HostModuleAPI(Router router)
        {
            _router = router;
        }

        public void FireEvent(CoreEvent coreEvent)
        {
            throw new NotImplementedException();
        }

        public string GenerateAuthToken(string entityID, string deviceID, int tokenExpirationMillis)
        {
            throw new NotImplementedException();
        }

        public string[] GetServerDomains()
        {
            throw new NotImplementedException();
        }

        public string GetServerID()
        {
            throw new NotImplementedException();
        }

        public void Hook(IC2SMessageHandler handler)
        {
            throw new NotImplementedException();
        }

        public void HookCoreEvent(ICoreEventHandler handler)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string connID, BaseMessage message)
        {
            throw new NotImplementedException();
        }

        public void Unhook(IC2SMessageHandler handler)
        {
            throw new NotImplementedException();
        }

        public void UnhookCoreEvent(ICoreEventHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}