using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Zirconium.Core.Models;
using Zirconium.Core.Modules.Interfaces;
using Zirconium.Utils;
using Zirconium.Core.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Zirconium.Core
{
    public class Router
    {
        private App _app;
        private IDictionary<string, List<IC2SMessageHandler>> _c2sMessageHandlers;
        private IDictionary<string, List<ICoreEventHandler>> _coreEventsHandlers;

        public Router(App app)
        {
            _app = app;
            _c2sMessageHandlers = new Dictionary<string, List<IC2SMessageHandler>>();
            _coreEventsHandlers = new Dictionary<string, List<ICoreEventHandler>>();
        }

        public void RouteMessage(ConnectionInfo connInfo, BaseMessage message)
        {
            var handlers = _c2sMessageHandlers[message.MessageType];
            if (handlers == null)
            {
                Log.Warning($"Drop message with type {message.MessageType} because server hasn't proper handlers");
                var serializedMsg = JsonConvert.SerializeObject(
                    OtherUtils.GenerateProtocolError(
                        message,
                        "unhandled",
                        $"Server doesn't implement message type {message.MessageType}",
                        new Dictionary<string, object>()
                    )
                );
                connInfo.ConnectionHandler.SendMessage(serializedMsg);
                return;
            }
            foreach (var h in handlers)
            {
                if (h.IsAuthorizationRequired())
                {
                    JWTPayload tokenPayload;
                    try
                    {
                        tokenPayload = _app.AuthManager.ValidateToken(message.AuthToken);
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e.Message);

                        var serializedMsg = JsonConvert.SerializeObject(
                            OtherUtils.GenerateProtocolError(
                                message,
                                "unauthorized",
                                "Unauthorized access",
                                new Dictionary<string, object>()
                            )
                        );
                        connInfo.ConnectionHandler.SendMessage(serializedMsg);
                        return;
                    }

                    if (connInfo.LastTokenHash == "" || connInfo.LastTokenHash == null)
                    {
                        string hash;
                        using (SHA512 shaM = new SHA512Managed())
                        {
                            hash = shaM.ComputeHash(message.AuthToken.ToByteArray()).ConvertToString();
                        }
                        connInfo.LastTokenHash = hash; 
                        // TODO implement comparing last token hash and if hash isn't changed then check payload
                        // if payload already exists - then skip validating auth token
                    }

                    connInfo.LastTokenPayload = tokenPayload;
                }

                Task.Run(() =>
                { 
                    // probably need to wrap whole foreach body, not only HandleMessage call - need to investigate
                    h.HandleMessage(message);
                });
            }
        }

        public void RouteCoreEvent(CoreEvent coreEvent)
        {
            var handlers = _coreEventsHandlers[coreEvent.Name];
            if (handlers == null) {
                Log.Warning($"Drop core event {coreEvent.Name} because server hasn't proper handlers");
                return;
            }
            foreach (var h in handlers) {
                Task.Run(() =>
                {
                    h.HandleEvent(coreEvent);
                });
            }
        }

        public void AddC2SHandler(string messageType, IC2SMessageHandler handler)
        {
            if (_c2sMessageHandlers.GetValueOrDefault(messageType, null) == null)
            {
                _c2sMessageHandlers[messageType] = new List<IC2SMessageHandler>();
            }
            this._c2sMessageHandlers[messageType].Add(handler);
        }

        public void AddCoreEventHandler(string eventType, ICoreEventHandler handler)
        {
            if (_coreEventsHandlers.GetValueOrDefault(eventType, null) == null)
            {
                _coreEventsHandlers[eventType] = new List<ICoreEventHandler>();
            }
            this._coreEventsHandlers[eventType].Add(handler);
        }
    }
}