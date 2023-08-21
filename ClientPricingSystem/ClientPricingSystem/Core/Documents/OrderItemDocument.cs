using ClientPricingSystem.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClientPricingSystem.Core.Documents;
public class OrderItemDocument
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid OrderId { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid VendorId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Size Size { get; set; }
    public int ArticleQuantity { get; set; }
    public decimal Total { get; set; }
    public decimal UnitPrice { get; set; }
    //public int InkQuantity { get; set; }
    //^^ should maybe look into making a DesignDetails object or something to store relevant design info at the order level.
    //   design info would be used to calculate cost (ink cost, etc.)
}

