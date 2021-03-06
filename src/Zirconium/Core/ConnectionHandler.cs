using System;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using WebSocketSharp;
using Zirconium.Core.Models;
using Zirconium.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Zirconium.Core
{
    public class ConnectionHandler : WebSocketBehavior
    {
        private App _app;

        public ConnectionHandler(App app)
        {
            _app = app;
        }

        protected override void OnClose(CloseEventArgs e)
        {
            _app.SessionManager.DeleteSession(ID);
            Log.Info($"Connection {ID} was closed (reason: {e.Reason})");
            // TODO implement closing connection
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Log.Error($"Error occurred: {e.Exception}");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                if (e.IsText)
                {
                    var msg = JsonConvert.DeserializeObject<BaseMessage>(e.Data);
                    _app.Router.RouteMessage(_app.SessionManager.GetSession(ID), msg);
                }
                else
                {
                    var errMsg = OtherUtils.GenerateProtocolError(
                        null,
                        "parseError",
                        $"Server cannot parse this message because it is not JSON",
                        new Dictionary<string, object>()
                    );
                    errMsg.From = _app.Config.ServerID;
                    var msgStr = JsonConvert.SerializeObject(errMsg);
                    this.SendMessage(msgStr);
                }
            }
            catch (Exception ex)
            {
                var errMsg = OtherUtils.GenerateProtocolError(
                    null,
                    "parseError",
                    $"Server cannot parse this message! {ex.Message}",
                    new Dictionary<string, object>()
                );
                errMsg.From = _app.Config.ServerID;
                var msgStr = JsonConvert.SerializeObject(errMsg);
                this.SendMessage(msgStr);
            }
        }

        protected override void OnOpen()
        {
            var ip = Context.UserEndPoint.Address;
            var session = new Session();
            session.ClientAddress = ip;
            session.ConnectionHandler = this;
            _app.SessionManager.AddSession(ID, session);
            Log.Info($"Connection {ID} was created");
        }

        public void SendMessage(string message)
        {
            this.Send(message);
        }

        public void SendMessage(BaseMessage message)
        {
            this.Send(JsonConvert.SerializeObject(message));
        }

        public void CloseConnection() {
            this.Sessions.CloseSession(this.ID); // TODO need to clarify if CloseSession also calls OnClose callback
        }
    }
}

