using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Numerics;

namespace ClientPricingSystem.Core.Methods.Vendor;

public class CreateVendor_FromDto
{
    public class Query : IRequest<Unit> 
    {
        public VendorDto VendorDto { get; set; }
    }

    public class Handler : IRequestHandler<Query, Unit>
    { 
        IMongoDatabase _context { get; set; }
        DatabaseConfiguration _config { get; set; }
        public Handler(IMongoClient mongoClient, IOptions<DatabaseConfiguration> config)
        {
            _context = mongoClient.GetDatabase(config.Value.DatabaseName);
            _config = config.Value;
        }

        public async Task<Unit> Handle(Query query, CancellationToken cancellationToken)
        {
            IMongoCollection<VendorDocument> vendorCollection = _context.GetCollection<VendorDocument>(_config.Vendors);

            VendorDocument vendor = VendorMapper.MapVendorDto_VendorDocument(query.VendorDto);
            await vendorCollection.InsertOneAsync(vendor).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
