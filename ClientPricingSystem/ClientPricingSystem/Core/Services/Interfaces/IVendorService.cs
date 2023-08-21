using ClientPricingSystem.Core.Documents;

namespace ClientPricingSystem.Core.Services;
public interface IVendorService
{
    public Task<List<VendorDocument>> GetAllVendorsAsync();
    public Task CreateVendorAsync(VendorDocument vendor);
}

