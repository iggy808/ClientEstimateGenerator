using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.Services;
public class VendorService : IVendorService
{
    IMongoDatabase _context;
    DatabaseConfiguration _config;

    public VendorService(IMongoClient mongoClient, IOptions<DatabaseConfiguration> config)
    {
        _context = mongoClient.GetDatabase(config.Value.DatabaseName);
        _config = config.Value;
    }

    public async Task<List<VendorDocument>> GetAllVendorsAsync()
    {
        IMongoCollection<VendorDocument> vendors = _context.GetCollection<VendorDocument>(_config.Vendors);
        return await vendors.Find(_ => true).ToListAsync().ConfigureAwait(false);
    }

    public async Task CreateVendorAsync(VendorDocument vendor)
    {
        IMongoCollection<VendorDocument> vendors = _context.GetCollection<VendorDocument>(_config.Vendors);
        await vendors.InsertOneAsync(vendor).ConfigureAwait(false);
    }
}

