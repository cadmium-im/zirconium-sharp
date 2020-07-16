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
            Logging.Log.Info($"Connection {ID} was closed (reason: {e.Reason})");
            // TODO implement closing connection
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Logging.Log.Error($"Error occurred: {e.Exception}");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                if (e.IsText)
                {
                    var msg = JsonConvert.DeserializeObject<BaseMessage>(e.Data);
                    _app.Router.RouteMessage(_app.SessionManager.GetConnectionInfo(ID), msg);
                }
                else
                {
                    var errMsg = OtherUtils.GenerateProtocolError(
                        null,
                        "parseError",
                        $"Server cannot parse this message yet because it is not JSON",
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
            var connInfo = new ConnectionInfo();
            connInfo.ClientAddress = ip;
            connInfo.ConnectionHandler = this;
            _app.SessionManager.AddSession(ID, connInfo);
            Logging.Log.Info($"Connection {ID} was created");
        }

        public void SendMessage(string message)
        {
            this.Send(message);
        }
    }
}

