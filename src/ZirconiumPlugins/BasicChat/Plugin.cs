using System.Linq;
using System;
using Newtonsoft.Json;
using Zirconium.Core.Models;
using Zirconium.Core.Plugins.Interfaces;
using Zirconium.Utils;
using Zirconium.Core.Logging;

namespace BasicChat
{
    internal class BasicChat : IPluginAPI
    {
        public string GetPluginUniqueName() => "BasicChat";

        public void Initialize(IPluginHostAPI pluginHost)
        {
            pluginHost.Hook(new ChatMessageRouterC2SHandler(pluginHost));
        }
    }

    internal class ChatMessageRouterC2SHandler : IC2SMessageHandler
    {
        class Message
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("content")]
            public Content Content { get; set; }
        }

        class Content
        {
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("media")]
            public Media[] Media { get; set; }
        }

        class Media
        {
            [JsonProperty("mimeType")]
            public string MimeType;

            [JsonProperty("url")]
            public string URL;
        }

        class MessageSentResponse : Message
        {
            [JsonProperty("messageID")]
            public string MessageID { get; set; }

            [JsonProperty("originServerTimestamp")]
            public long OriginServerTimestamp { get; set; }
        }

        private IPluginHostAPI pluginHostAPI;

        public ChatMessageRouterC2SHandler(IPluginHostAPI pluginHostAPI)
        {
            this.pluginHostAPI = pluginHostAPI;
        }

        public string GetHandlerUniqueID() => "ChatMessageRouterC2SHandler";

        public string GetHandlingMessageType() => "urn:cadmium:chats:message";

        public void HandleMessage(Session session, BaseMessage message)
        {
            var receivedMessage = JsonConvert.DeserializeObject<Message>(JsonConvert.SerializeObject(message.Payload));
            var response = new BaseMessage(message, true);
            response.Ok = true;
            response.From = pluginHostAPI.GetServerID();
            response.To = null;
            var respPayload = new MessageSentResponse();
            respPayload.MessageID = Guid.NewGuid().ToString();
            respPayload.OriginServerTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            response.Payload = respPayload.ToDictionary();
            session.ConnectionHandler.SendMessage(response);

            var recipientSession = pluginHostAPI.GetSessionManager()
                .GetSessions()
                .Select(x => x.Value)
                .Where(x => x.LastTokenPayload.EntityID.Where(x => x == message.To.First()).FirstOrDefault() != null)
                .FirstOrDefault();

            if (recipientSession == null)
            {
                Log.Debug("Recipient doesn't exist or currently offline!");
                return; // TODO fix this behaviour in specs
            }

            var msgForRecipient = new BaseMessage();
            msgForRecipient.From = session.LastTokenPayload.EntityID.First();
            msgForRecipient.MessageType = "urn:cadmium:chats:message";
            respPayload.Type = receivedMessage.Type;
            respPayload.Content = receivedMessage.Content;
            msgForRecipient.Payload = respPayload.ToDictionary();
            recipientSession.ConnectionHandler.SendMessage(msgForRecipient);
        }

        public bool IsAuthorizationRequired() => true;
    }
}
