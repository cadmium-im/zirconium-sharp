using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Zirconium.Core.Models;
using Zirconium.Core.Plugins.Interfaces;
using Zirconium.Utils;
using Log4Sharp;
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

        public void RouteMessage(Session session, BaseMessage message)
        {
            var handlers = _c2sMessageHandlers.GetValueOrDefault(message.MessageType, null);
            if (handlers == null)
            {
                Log.Warning($"Drop message with type \"{message.MessageType}\" because server hasn't proper handlers");
                var msg = OtherUtils.GenerateProtocolError(
                    message,
                    "unhandled",
                    $"Server doesn't implement message type \"{message.MessageType}\"",
                    new Dictionary<string, object>()
                );
                msg.From = _app.Config.ServerID;
                session.ConnectionHandler.SendMessage(msg);
                return;
            }
            var handlerTasks = new List<Task>();
            foreach (var h in handlers)
            {
                if (h.IsAuthorizationRequired() && session.AuthData == null)
                {
                    session.ConnectionHandler.SendMessage(OtherUtils.GenerateUnauthorizedError(message, _app.Config.ServerID));
                    return;
                }
                
                handlerTasks.Add(Task.Run(() =>
                {
                    // probably need to wrap whole foreach body, not only HandleMessage call - need to investigate
                    h.HandleMessage(session, message);
                }));
            }
            try
            {
                Task.WaitAll(handlerTasks.ToArray());
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        public void RouteCoreEvent(CoreEvent coreEvent)
        {
            var handlers = _coreEventsHandlers[coreEvent.Name];
            if (handlers == null)
            {
                Log.Warning($"Drop core event {coreEvent.Name} because server hasn't proper handlers");
                return;
            }
            foreach (var h in handlers)
            {
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

        public void RemoveC2SHandler(string messageType, IC2SMessageHandler handler)
        {
            if (!this._c2sMessageHandlers[messageType].Remove(handler))
            {
                Log.Warning("attempt to remove c2s handler which doesn't exist in router");
            }
        }

        public void AddCoreEventHandler(string eventType, ICoreEventHandler handler)
        {
            if (_coreEventsHandlers.GetValueOrDefault(eventType, null) == null)
            {
                _coreEventsHandlers[eventType] = new List<ICoreEventHandler>();
            }
            this._coreEventsHandlers[eventType].Add(handler);
        }

        public void RemoveCoreEventHandler(string eventType, ICoreEventHandler handler)
        {
            if (!this._coreEventsHandlers[eventType].Remove(handler))
            {
                Log.Warning("attempt to remove core handler which doesn't exist in router");
            }
        }
    }
}