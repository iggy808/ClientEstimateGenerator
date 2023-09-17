using ClientPricingSystem.Configuration;
using ClientPricingSystem.Configuration.Mapper;
using ClientPricingSystem.Core.Documents;
using ClientPricingSystem.Core.Dtos;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClientPricingSystem.Core.MediatRMethods.Vendor;
public class CreateVendor_FromDto
{
    public class Command : IRequest<Unit> 
    {
        public VendorDto VendorDto { get; set; }
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        IMongoDatabase _context;
        DatabaseConfiguration _config;
        public Handler(IMongoClient mongoClient, IOptions<DatabaseConfiguration> config)
        {
            _context = mongoClient.GetDatabase(config.Value.DatabaseName);
            _config = config.Value;
        }

        public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
        {
            IMongoCollection<VendorDocument> vendorCollection = _context.GetCollection<VendorDocument>(_config.Vendors);

            VendorDocument vendor = VendorMapper.MapVendorDto_VendorDocument(command.VendorDto);
            if (command.VendorDto.Domains != null)
                vendor.Domains = command.VendorDto.Domains;
            
            await vendorCollection.InsertOneAsync(vendor).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
