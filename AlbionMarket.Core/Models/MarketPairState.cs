using AlbionMarket.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlbionMarket.Core.Models
{
    public class MarketPairState
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Tier + Name + Ench
        public string ItemId { get; set; }

        public int Quality { get; set; }

        /// <summary>
        /// Time when MP Status was updated last time
        /// </summary>
        public DateTime? StatusUpdatedAt { get; set; }

        public MarketPairStatus Status { get; set; }

        /// <summary>
        /// Last profit value, to deside whether we should display the itenm or not
        /// </summary>
        public int? LastProfit { get; set; }
    }
}
