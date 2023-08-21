using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace ClientPricingSystem.Configuration.Mapper;

[Mapper(AllowNullPropertyAssignment = true)]
public static partial class VendorMapper
{
    [MapProperty(nameof(VendorDocument.Domains), nameof(VendorDto.DomainsRaw))]
    public static partial VendorDto MapVendorDocument_VendorDto(VendorDocument vendorDocument);

    [MapProperty(nameof(VendorDto.DomainsRaw), nameof(VendorDocument.Domains))]
    public static partial VendorDocument MapVendorDto_VendorDocument(VendorDto vendorDto);
    public static List<string> ToDomainsList(string domainsRaw) => domainsRaw.Split(", ").ToList();
    public static string ToDomainsRaw(List<string> domains) => String.Join(", ", domains);
}