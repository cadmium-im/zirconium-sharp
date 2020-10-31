using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Zirconium.Core.Models;
using Zirconium.Core.Plugins.Interfaces;
using Zirconium.Utils;

namespace InBandLogin
{
    public class InBandLoginPlugin : IPluginAPI
    {
        class LoginRequestPayload
        {
            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }
        }

        class LoginResponsePayload
        {
            [JsonProperty("authToken")]
            public string AuthToken { get; set; }

            [JsonProperty("deviceID")]
            public string DeviceID { get; set; }
        }

        class RegisterRequestPayload
        {
            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("loginOnSuccess")]
            public bool LoginOnSuccess { get; set; }
        }

        class RegisterResponsePayload
        {
            [JsonProperty("userID")]
            public string UserID { get; set; }

            [JsonProperty("authToken")]
            public string AuthToken { get; set; }

            [JsonProperty("deviceID")]
            public string DeviceID { get; set; }
        }

        private IPluginHostAPI pluginHostAPI;

        public string GetPluginUniqueName()
        {
            return "InBandLogin";
        }

        public void Initialize(IPluginHostAPI pluginHost)
        {
            this.pluginHostAPI = pluginHost;
            pluginHostAPI.Hook(new LoginC2SHandler(pluginHostAPI));
            pluginHostAPI.Hook(new RegisterC2SHandler(pluginHostAPI));
        }

        class LoginC2SHandler : IC2SMessageHandler
        {
            private const string errID = "invalid_creds";
            private IPluginHostAPI _phostAPI;
            public LoginC2SHandler(IPluginHostAPI pluginHostAPI)
            {
                this._phostAPI = pluginHostAPI;
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
                var authProvider = _phostAPI.GetAuthProvider();
                if (authProvider.TestPassword(pObj.Username, pObj.Password))
                {
                    BaseMessage reply = new BaseMessage(message, true);
                    var p = new LoginResponsePayload();
                    string deviceID = "ABCDEF"; // TODO fix device id system
                    p.AuthToken = _phostAPI.GenerateAuthToken($"@{pObj.Username}@{_phostAPI.GetServerID()}", deviceID);
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

        class RegisterC2SHandler : IC2SMessageHandler
        {

            private IPluginHostAPI _pluginHostAPI;

            public RegisterC2SHandler(IPluginHostAPI pluginHostAPI)
            {
                this._pluginHostAPI = pluginHostAPI;
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
                var authProvider = _pluginHostAPI.GetAuthProvider();
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
                p.UserID = $"@{pObj.Username}@{_pluginHostAPI.GetServerID()}";
                if (pObj.LoginOnSuccess)
                {
                    string deviceID = "ABCDEF"; // TODO fix device id system
                    p.AuthToken = _pluginHostAPI.GenerateAuthToken($"@{pObj.Username}@{_pluginHostAPI.GetServerID()}", deviceID);
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
}
