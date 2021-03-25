using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatSubsystem.Storage.Interfaces;
using ChatSubsystem.Storage.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Zirconium.Core.Models;

namespace ChatSubsystem.Storage
{
    public class EventStorageManager : IEventStorageManager
    {
        private const string EventsCollectionName = "events";
        
        private ChatStorageManager _chatStorageManager;
        private IMongoCollection<Event> events;

        public EventStorageManager(ChatStorageManager chatStorageManager, IMongoDatabase database)
        {
            _chatStorageManager = chatStorageManager;
            events = database.GetCollection<Event>(EventsCollectionName);
        }

        public async Task<(IList<Event>, IList<Chat>)> GetEventsForUser(EntityID user, EntityID since, int limit)
        {
            var chats = await _chatStorageManager.GetChatsForUser(user);

            var evs = new List<Event>();

            foreach (var c in chats)
            {
                var res = await GetEventsForChat(c, since, limit);
                evs.AddRange(res);
            }

            evs = evs.OrderBy(x => x.Timestamp).ToList();

            return (evs, chats);
        }

        private async Task<IList<Event>> GetEventsForChat(Chat chat, EntityID since, int limit)
        {
            var graphLookupStage = new BsonDocument("$graphLookup",
                new BsonDocument
                {
                    { "from", EventsCollectionName },
                    { "startWith", "{ EventID: "+since+" }" },
                    { "connectFromField", "EventID"},
                    { "connectToField",  "PrevEvents" },
                    { "as", "Children" },
                    { "maxDepth", limit },
                    { "restrictSearchWithMatch", "{ ChatId: "+chat.Id+" }" }
                });

            var result = await events.Aggregate()
                .AppendStage<BsonDocument>(graphLookupStage).ToListAsync();

            var deserializedResult = new List<Event>();
            result.ForEach(x =>
            {
                deserializedResult.Add(BsonSerializer.Deserialize<EventWithChildren>(x));
            });

            return deserializedResult;
        }

        public async Task SaveEvent(Event e)
        {
            var graphLookupStage = new BsonDocument("$graphLookup",
                new BsonDocument
                {
                    { "from", EventsCollectionName },
                    { "startWith", "$_id" },
                    { "connectFromField", "_id"},
                    { "connectToField",  "PrevID" },
                    { "as", "Children" },
                    { "maxDepth", 0 }
                });
            
            var result = await events.Aggregate()
                .AppendStage<BsonDocument>(graphLookupStage)
                .Match("{ Children: { $size: 0 } }")
                .ToListAsync();
            
            if (result.Count == 0)
            {
                await events.InsertOneAsync(e);
                return;
            }
            result.ForEach(x =>
            {
                var y = BsonSerializer.Deserialize<EventWithChildren>(x);
                e.PrevID.Append(y.Id);
            });
            
            graphLookupStage = new BsonDocument("$graphLookup",
                new BsonDocument
                {
                    { "from", EventsCollectionName },
                    { "startWith", "$EventID" },
                    { "connectFromField", "EventID"},
                    { "connectToField",  "PrevEvents" },
                    { "as", "Children" },
                    { "maxDepth", 0 },
                    { "restrictSearchWithMatch", "{ ChatId: "+e.ChatId+" }" }
                });

            
            result = await events.Aggregate()
                .AppendStage<BsonDocument>(graphLookupStage)
                .Match("{ Children: { $size: 0 } }")
                .ToListAsync();
            
            if (result.Count == 0)
            {
                await events.InsertOneAsync(e);
                return;
            }
            result.ForEach(x =>
            {
                var y = BsonSerializer.Deserialize<EventWithChildren>(x);
                e.PrevEvents.Append(y.EventID);
            });
            
            await events.InsertOneAsync(e);
        }

        public async Task<Event> GetEventById(EntityID id)
        {
            var filter =
                Builders<Event>.Filter.Where(
                    x => x.Id.ToString() == id.LocalPart && x.OriginServer.ServerPart == id.ServerPart);
            var res = await events.FindAsync(filter);
            var ev = res.FirstOrDefault();
            return ev;
        }
    }
}