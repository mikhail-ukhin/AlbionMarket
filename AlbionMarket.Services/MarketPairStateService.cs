using AlbionMarket.Core.Configuration;
using AlbionMarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlbionMarket.Services
{
    public class MarketPairStateService
    {
        private readonly AlbionDatabaseSettings _albionDatabaseSettings;
        private readonly IMongoCollection<MarketPairState> _marketPairStateCollection;

        public MarketPairStateService(IOptions<AlbionDatabaseSettings> options)
        {
            _albionDatabaseSettings = options.Value;

            var mongoClient = new MongoClient(_albionDatabaseSettings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(_albionDatabaseSettings.DatabaseName);

            _marketPairStateCollection = mongoDatabase.GetCollection<MarketPairState>(_albionDatabaseSettings.CollectionName);
        }

        public async Task<List<MarketPairState>> GetAsync() => await _marketPairStateCollection.Find(_ => true).ToListAsync();

        public async Task<MarketPairState?> GetAsync(string id) => await _marketPairStateCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(MarketPairState newMarketPairState) => await _marketPairStateCollection.InsertOneAsync(newMarketPairState);

        public async Task UpdateAsync(string id, MarketPairState updatedMarketPairState) => await _marketPairStateCollection.ReplaceOneAsync(x => x.Id == id, updatedMarketPairState);

        public async Task RemoveAsync(string id) => await _marketPairStateCollection.DeleteOneAsync(x => x.Id == id);
    }
}
