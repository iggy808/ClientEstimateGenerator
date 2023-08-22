using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.Methods.Vendor;

public class GetAllVendors_ToDto
{
    public class Query : IRequest<VendorDto> {}

    public class Handler : IRequestHandler<Query, VendorDto>
    { 
        IMongoDatabase _context { get; set; }
        DatabaseConfiguration _config { get; set; }
        public Handler(IMongoClient mongoClient, IOptions<DatabaseConfiguration> config)
        {
            _context = mongoClient.GetDatabase(config.Value.DatabaseName);
            _config = config.Value;
        }

        public async Task<VendorDto> Handle(Query query, CancellationToken cancellationToken)
        {
            IMongoCollection<VendorDocument> vendorCollection = _context.GetCollection<VendorDocument>(_config.Vendors);
            List<VendorDocument> vendors = await vendorCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);

            VendorDto vendorDto = new VendorDto
            {
                Vendors = vendors.Select(v => VendorMapper.MapVendorDocument_VendorDto(v)).ToList()
            };

            return vendorDto;
        }
    }
}
