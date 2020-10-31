using System.Collections.Generic;
using Zirconium.Core.Models;
using Zirconium.Core.Plugins.Interfaces;
using Zirconium.Utils;

namespace InBandLogin.Handlers
{
    public class LoginC2SHandler : IC2SMessageHandler
    {
        private const string errID = "invalid_creds";
        private readonly IPluginHostAPI _pluginHost;

        public LoginC2SHandler(IPluginHostAPI pluginHostApi)
        {
            this._pluginHost = pluginHostApi;
        }

        public string GetHandlerUniqueID()
        {
            return "LoginC2SHandler";
        }

        public string GetHandlingMessageType()
        {
            return "profile:login";
        }

        public void HandleMessage(Session session, BaseMessage message)
        {
            var pObj = message.Payload.ToObject<LoginRequestPayload>();
            var authProvider = _pluginHost.GetAuthProvider();
            if (authProvider.TestPassword(pObj.Username, pObj.Password))
            {
                BaseMessage reply = new BaseMessage(message, true);
                var p = new LoginResponsePayload();
                string deviceID = "ABCDEF"; // TODO fix device id system
                p.AuthToken = _pluginHost.GenerateAuthToken($"@{pObj.Username}@{_pluginHost.GetServerID()}", deviceID);
                p.DeviceID = deviceID;
                reply.Payload = p.ToDictionary();
                reply.Ok = true;
                session.ConnectionHandler.SendMessage(reply);
            }
            else
            {
                var reply = OtherUtils.GenerateProtocolError(
                    message,
                    errID,
                    "Username/password isn't valid",
                    new Dictionary<string, object>()
                );
                session.ConnectionHandler.SendMessage(reply);
            }
        }

        public bool IsAuthorizationRequired()
        {
            return false;
        }
    }
}