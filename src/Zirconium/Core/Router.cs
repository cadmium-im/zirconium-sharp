using System.Collections.Generic;
using Zirconium.Core.Models;
using Zirconium.Core.Modules.Interfaces;
using Zirconium.Utils;

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

        public void RouteMessage(ConnectionInfo connInfo, BaseMessage message) {

        }

        public void AddC2SHandler(string messageType, IC2SMessageHandler handler) {
            if (_c2sMessageHandlers.GetValueOrDefault(messageType, null) == null) {
                _c2sMessageHandlers[messageType] = new List<IC2SMessageHandler>();
            }
            this._c2sMessageHandlers[messageType].Add(handler);
        }

        public void AddCoreEventHandler(string eventType, ICoreEventHandler handler) {
            if (_coreEventsHandlers.GetValueOrDefault(eventType, null) == null) {
                _coreEventsHandlers[eventType] = new List<ICoreEventHandler>();
            }
            this._coreEventsHandlers[eventType].Add(handler);
        }
    }
}