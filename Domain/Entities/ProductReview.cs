using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Domain.Entitties
{
    public class ProductReview
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        
        [BsonRepresentation(BsonType.String)]
        public Guid BookId { get; set; } = Guid.NewGuid();

        public string ReviewerName { get; set; } = string.Empty;

        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

        public DateTime Date {  get; set; } = DateTime.Now;
 
    }
}


