using System;
using System.Collections.Generic;
using Zirconium.Core.Models;
using Zirconium.Core.Plugins.Interfaces;
using Zirconium.Utils;

namespace InBandLogin.Handlers
{
    public class RegisterC2SHandler : IC2SMessageHandler
    {
        private readonly IPluginHostAPI _pluginHost;

        public RegisterC2SHandler(IPluginHostAPI pluginHost)
        {
            this._pluginHost = pluginHost;
        }

        public string GetHandlerUniqueID()
        {
            return "RegisterC2SHandler";
        }

        public string GetHandlingMessageType()
        {
            return "profile:register";
        }

        public void HandleMessage(Session session, BaseMessage message)
        {
            var pObj = message.Payload.ToObject<RegisterRequestPayload>();
            var authProvider = _pluginHost.GetAuthProvider();
            try
            {
                authProvider.CreateUser(pObj.Username, pObj.Password);
            }
            catch (Exception e)
            {
                BaseMessage errorReply;
                if (e.Message.Contains("E11000"))
                {
                    errorReply = OtherUtils.GenerateProtocolError(
                        message,
                        "id_exists",
                        "Username already taken",
                        new Dictionary<string, object>()
                    );
                }
                else
                {
                    errorReply = OtherUtils.GenerateProtocolError(
                        message,
                        "other",
                        e.ToString(),
                        new Dictionary<string, object>()
                    );
                }

                session.ConnectionHandler.SendMessage(errorReply);
                return;
            }

            BaseMessage reply = new BaseMessage(message, true);
            var p = new RegisterResponsePayload();
            p.UserID = $"@{pObj.Username}@{_pluginHost.GetServerID()}";
            if (pObj.LoginOnSuccess)
            {
                string deviceID = "ABCDEF"; // TODO fix device id system
                p.AuthToken = _pluginHost.GenerateAuthToken($"@{pObj.Username}@{_pluginHost.GetServerID()}", deviceID);
                p.DeviceID = deviceID;
            }

            reply.Payload = p.ToDictionary();
            reply.Ok = true;
            session.ConnectionHandler.SendMessage(reply);
        }

        public bool IsAuthorizationRequired()
        {
            return false;
        }
    }
}