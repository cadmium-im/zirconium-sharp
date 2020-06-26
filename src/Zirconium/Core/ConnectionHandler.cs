using System;
using WebSocketSharp.Server;
using WebSocketSharp;
using Zirconium.Core.Models;
using Zirconium.Utils;

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
            Console.WriteLine($"Connection {ID} was closed (reason: {e.Reason})"); // TODO implement normal logging
            // TODO implement closing connection
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine($"Error occurred: {e.Exception}"); // TODO implement normal logging
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            // TODO implement message parsing and routing
        }

        protected override void OnOpen()
        {
            var ip = Context.UserEndPoint.Address;
            var connInfo = new ConnectionInfo();
            connInfo.ClientAddress = ip;
            connInfo.ConnectionHandler = this;
            _app.SessionManager.AddSession(ID, connInfo);
            Console.WriteLine($"Connection {ID} was created"); // TODO implement normal logging
        }

        public void SendMessage(string message) {
            this.Send(message.ToByteArray());
        }
    }
}

