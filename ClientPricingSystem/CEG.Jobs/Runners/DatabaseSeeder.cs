using CEG.Jobs.Configuration;
using CEG.Jobs.Fakers;
using ClientPricingSystem.Core.Documents;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace CEG.Jobs.Runners;
public class DatabaseSeeder
{
    DatabaseConfiguration? _dbConfig;
    MongoClient? _mongoClient;
    IMongoDatabase _testDatabase;

    const int TestClientRecordsCount = 50;
    const int TestVendorRecordsCount = 10;
    const int TestOrderRecordsCount = 200;
    const decimal TAX = 10.00m;
    public void EstablishConnectionToDatabase()
    {
        IConfigurationRoot jobsConfig = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        DatabaseConfiguration? dbConfig = jobsConfig.GetRequiredSection("DatabaseConfiguration").Get<DatabaseConfiguration>();

        if (dbConfig != null)
        {
            dbConfig.DefaultConnectionString = jobsConfig.GetSection("ConnectionStrings").GetValue<string>("DefaultConnectionString");
            _dbConfig = dbConfig;
            _mongoClient = new MongoClient(dbConfig.DefaultConnectionString);
        }
    }

    public string SeedTestDatabase()
    {
        EstablishConnectionToDatabase();

        if (_mongoClient == null || _dbConfig == null)
            return "Mongo client could not be created, check CEG.Jobs/appsettings.json to verify connection info is configured correctly.";

        WipeTestDatabase();

        SeedClientCollection();

        SeedVendorCollection();

        SeedOrderCollection();

        return "Test database successfully seeded.";
    }

    void WipeTestDatabase()
    {
        if (_mongoClient == null || _dbConfig == null)
            return;

        _mongoClient.DropDatabase(_dbConfig.DatabaseName);
        _testDatabase = _mongoClient.GetDatabase(_dbConfig.DatabaseName);
    }

    void SeedClientCollection()
    {
        IMongoCollection<ClientDocument> clientCollection = _testDatabase.GetCollection<ClientDocument>(_dbConfig?.Clients);

        List<ClientDocument> clients = ClientFaker.GetClientFaker().Generate(TestClientRecordsCount);

        clientCollection.InsertMany(clients);
    }

    void SeedVendorCollection()
    {
        IMongoCollection<VendorDocument> vendorCollection = _testDatabase.GetCollection<VendorDocument>(_dbConfig?.Vendors);

        List<VendorDocument> vendors = VendorFaker.GetVendorFaker().Generate(TestVendorRecordsCount);

        vendorCollection.InsertMany(vendors);
    }

    void SeedOrderCollection()
    {
        IMongoCollection<ClientDocument> clientCollection = _testDatabase.GetCollection<ClientDocument>(_dbConfig?.Clients);
        IMongoCollection<OrderDocument> orderCollection = _testDatabase.GetCollection<OrderDocument>(_dbConfig?.Orders);
        IMongoCollection<VendorDocument> vendorCollection = _testDatabase.GetCollection<VendorDocument>(_dbConfig?.Vendors);
        IMongoCollection<OrderItemDocument> orderItemCollection = _testDatabase.GetCollection<OrderItemDocument>(_dbConfig?.OrderItems);

        List<VendorDocument> vendors = vendorCollection.Find(_ => true).ToList();
        List<ClientDocument> clients = clientCollection.Find(_ => true).ToList();

        List<OrderDocument> orders = OrderFaker
            .GetOrderFaker(clients, vendors, TestClientRecordsCount)
            .Generate(TestOrderRecordsCount);

        List<OrderItemDocument> orderItems = new List<OrderItemDocument>();
        foreach (OrderDocument order in orders)
        {
            order.ClientId = order.Client.Id;
            
            if (order.Items == null)
                continue;

            decimal subtotal = 0.00m;
            foreach (OrderItemDocument item in order.Items)
            {
                item.OrderId = order.Id;
                orderItems.Add(item);
                subtotal += item.UnitPrice * item.ArticleQuantity;
            }

            order.SubTotal = Math.Round(subtotal, 2);
            order.Total = Math.Round(subtotal + TAX, 2);
        }

        orderCollection.InsertMany(orders);
        orderItemCollection.InsertMany(orderItems);
    }
}