using System;
using Newtonsoft.Json;
using Log4Sharp;
using Zirconium.Core.Models;
using Zirconium.Core.Plugins.Interfaces;

namespace HelloWorldPlugin
{
    internal class HelloWorldPlugin : IPluginAPI
    {
        public string GetPluginUniqueName() => "HelloWorldPlugin";

        public void Initialize(IPluginHostAPI hostModuleAPI)
        {
            var handler = new C2SHandler(hostModuleAPI);
            hostModuleAPI.Hook(handler);
            Log.Debug("plugin is initialized");
        }
    }

    internal class C2SHandler : IC2SMessageHandler
    {
        private IPluginHostAPI hostModuleAPI;

        public C2SHandler(IPluginHostAPI hostModuleAPI)
        {
            this.hostModuleAPI = hostModuleAPI;
        }

        public string GetHandlerUniqueID() => "test";

        public string GetHandlingMessageType() => "test";

        public void HandleMessage(Session session, BaseMessage message)
        {
            BaseMessage msg = new BaseMessage(message, true);
            msg.Payload["testProp"] = "hello world";
            msg.Ok = true;
            msg.From = hostModuleAPI.GetServerID();
            session.ConnectionHandler.SendMessage(msg);
        }

        public bool IsAuthorizationRequired() => false;
    }
}
