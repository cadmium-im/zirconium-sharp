using System;
using Newtonsoft.Json;
using Zirconium.Core.Models;
using Zirconium.Core.Modules.Interfaces;

namespace Zirconium.Core.Modules
{
    public class HostModuleAPI : IHostModuleAPI
    {
        private App _app;
        private Router _router;

        public HostModuleAPI(App app, Router router)
        {
            _router = router;
            _app = app;
        }

        public void FireEvent(CoreEvent coreEvent)
        {
            _router.RouteCoreEvent(coreEvent);
        }

        public string GenerateAuthToken(string entityID, string deviceID, int tokenExpirationMillis)
        {
            return _app.AuthManager.CreateToken(entityID, deviceID, tokenExpirationMillis);
        }

        public string[] GetServerDomains()
        {
            return _app.Config.ServerDomains;
        }

        public string GetServerID()
        {
            return _app.Config.ServerID;
        }

        public void Hook(IC2SMessageHandler handler)
        {
            _router.AddC2SHandler(handler.GetHandlingMessageType(), handler);
        }

        public void HookCoreEvent(ICoreEventHandler handler)
        {
            _router.AddCoreEventHandler(handler.GetHandlingEventType(), handler);
        }

        public void SendMessage(ConnectionInfo connInfo, BaseMessage message)
        {
            connInfo.ConnectionHandler.SendMessage(JsonConvert.SerializeObject(message));
        }

        public void Unhook(IC2SMessageHandler handler)
        {
            _router.RemoveC2SHandler(handler.GetHandlingMessageType(), handler);
        }

        public void UnhookCoreEvent(ICoreEventHandler handler)
        {
            _router.RemoveCoreEventHandler(handler.GetHandlingEventType(), handler);
        }
    }
}