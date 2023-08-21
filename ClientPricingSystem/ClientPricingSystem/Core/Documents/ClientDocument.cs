using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClientPricingSystem.Core.Documents;
public class ClientDocument
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }

    public decimal MarkupRate { get; set; }
}

