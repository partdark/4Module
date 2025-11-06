

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnalyticsWorker.Models
{
    public class BookEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set;  }

        public string Key { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
