using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClientPricingSystem.Core.Documents;
public class VendorDocument
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
    public List<string>? Domains { get; set; }
}