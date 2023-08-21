using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClientPricingSystem.Core.Documents;
public class OrderDocument
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid ClientId { get; set; }
    public ClientDocument Client { get; set; }
    public decimal ArtistFee { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }

    // Note: Items are not stored alongside the Orders in the table,
    //       they are stored in the OrderItems table,
    //       and are fetched by OrderId when needed.
    [BsonIgnore]
    public List<OrderItemDocument>? Items { get; set; }
}

