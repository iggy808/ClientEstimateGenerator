namespace ClientPricingSystem.Core.Dtos;
public class VendorDto
{
    // Document fields
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Notes { get; set; }
    public List<string> Domains { get; set; }

    // View fields
    public string DomainsRaw { get; set; }
    public List<VendorDto>? Vendors { get; set; }
}
