using Newtonsoft.Json;
using Zirconium.Core.Logging;
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

        public C2SHandler(IPluginHostAPI hostModuleAPI) {
            this.hostModuleAPI = hostModuleAPI;
        }

        public string GetHandlerUniqueID() => "test";

        public string GetHandlingMessageType() => "test";

        public void HandleMessage(ConnectionInfo connInfo, BaseMessage message)
        {
            BaseMessage msg = new BaseMessage(message, true);
            msg.Payload["testProp"] = "hello world";
            msg.Ok = true;
            msg.From = hostModuleAPI.GetServerID();
            string strMsg = JsonConvert.SerializeObject(msg);
            connInfo.ConnectionHandler.SendMessage(strMsg);
        }

        public bool IsAuthorizationRequired() => false;
    }
}
