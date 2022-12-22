using AlbionMarket.Core.Configuration;
using AlbionMarket.Core.Enums;
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

        public async Task<List<MarketPairState>> GetAllAsync() => await _marketPairStateCollection.Find(_ => true).ToListAsync();

        public Task<MarketPairState> GetAsync(string id) => _marketPairStateCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public Task<MarketPairState> GetAsync(string itemId, int quality) => _marketPairStateCollection.Find(x => x.ItemId == itemId && x.Quality == quality).FirstOrDefaultAsync();

        public async Task CreateAsync(MarketPairState newMarketPairState) => await _marketPairStateCollection.InsertOneAsync(newMarketPairState);

        public async Task UpdateAsync(string id, MarketPairState updatedMarketPairState) => await _marketPairStateCollection.ReplaceOneAsync(x => x.Id == id, updatedMarketPairState);

        public void Update(string id, MarketPairState updatedMarketPairState) => _marketPairStateCollection.ReplaceOne(x => x.Id == id, updatedMarketPairState);

        public async Task RemoveAsync(string id) => await _marketPairStateCollection.DeleteOneAsync(x => x.Id == id);

        public void UpdateMarketPairStatus(string itemId, int quality, MarketPairStatus status)
        {
            var marketPairState = _marketPairStateCollection.Find(p => p.ItemId == itemId && p.Quality == quality).FirstOrDefault();

            if (marketPairState == null) return;

            marketPairState.Status = status;
            marketPairState.StatusUpdatedAt = DateTime.UtcNow;

            Update(marketPairState.Id, marketPairState);
        }

        public void RemoveAll()
        {
            _marketPairStateCollection.DeleteMany(_ => true);
        }
    }
}
