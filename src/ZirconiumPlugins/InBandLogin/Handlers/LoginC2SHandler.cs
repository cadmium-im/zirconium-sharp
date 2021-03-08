using System.Collections.Generic;
using System.Linq;
using Zirconium.Core.Models;
using Zirconium.Core.Models.Authorization;
using Zirconium.Core.Plugins.Interfaces;
using Zirconium.Utils;

namespace InBandLogin.Handlers
{
    public class LoginC2SHandler : IC2SMessageHandler
    {
        private const string errID = "urn:cadmium:auth:invalid";
        private const string invalidAuthType = "urn:cadmium:auth:invalid_type";
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
            return "urn:cadmium:auth";
        }

        public void HandleMessage(Session session, BaseMessage message)
        {
            var pObj = message.Payload.ToObject<AuthorizationRequest>();
            var authProvider = _pluginHost.GetAuthProvider();
            if (!authProvider.GetAuthSupportedMethods().Contains(pObj.Type))
            {
                var reply = OtherUtils.GenerateProtocolError(
                    message,
                    invalidAuthType,
                    "auth type is invalid"
                );
                session.ConnectionHandler.SendMessage(reply);
                return;
            }
            var authData = authProvider.TestAuthFields(pObj.Fields);
            if (authData.Item1 != null)
            {
                BaseMessage reply = new BaseMessage(message, true);
                if (authData.Item2 != null)
                {
                    reply.Payload = authData.Item2.ToDictionary();
                }
                reply.Ok = true;
                session.ConnectionHandler.SendMessage(reply);
                session.AuthData = authData.Item1;
            }
            else
            {
                var reply = OtherUtils.GenerateProtocolError(
                    message,
                    errID,
                    "auth credentials isn't valid"
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