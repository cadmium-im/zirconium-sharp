using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatSubsystem.Storage.Models;
using MongoDB.Driver;
using Zirconium.Core.Models;

namespace ChatSubsystem.Storage
{
    public class ChatStorageManager
    {
        // get private chat by user 1 and user 2
        // get chat by id
        // update chat

        private const string ChatsCollectionName = "chats";

        private IMongoCollection<Chat> chatsCollection;
        
        public ChatStorageManager(IMongoDatabase database)
        {
            chatsCollection = database.GetCollection<Chat>(ChatsCollectionName);
        }

        public async Task<Chat> GetPrivateChat(EntityID u1, EntityID u2)
        {
            var filter =
                Builders<Chat>.Filter.Where(
                    x => x.Members.Contains(u1.ToString()) && x.Members.Contains(u2.ToString()) && x.Members.Length == 2 && x.IsPrivateChat);
            var res = await chatsCollection.FindAsync(filter);
            var chat = res.FirstOrDefault();
            return chat;
        }

        public async Task<Chat> GetById(EntityID chatId)
        {
            var filter =
                Builders<Chat>.Filter.Where(
                    x => x.ChatID == chatId.ToString());
            var res = await chatsCollection.FindAsync(filter);
            var chat = res.FirstOrDefault();
            return chat;
        }

        public async Task<IList<Chat>> GetChatsForUser(EntityID user)
        {
            var filter =
                Builders<Chat>.Filter.Where(
                    x => x.Members.Contains(user.ToString()));
            var res = await chatsCollection.FindAsync(filter);
            var chat = res.ToList();
            return chat;
        }
    }
}