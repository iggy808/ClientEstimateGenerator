using CEG.Jobs.Configuration;
using ClientPricingSystem.Core.Documents;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace CEG.Jobs.Runners;
public class DatabaseSeeder
{
    DatabaseConfiguration? _dbConfig;
    MongoClient? _mongoClient;
    IMongoDatabase _testDatabase;
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
        IMongoCollection<ClientDocument> clientCollection = _testDatabase.GetCollection<ClientDocument>(_dbConfig.Clients);
        clientCollection.InsertOne(new ClientDocument { Name = "LA Tech Soccer", Address = "921 Tech Dr, Ruston, LA 71270", MarkupRate = 60.00m });
    }

    void SeedVendorCollection()
    {

    }

    void SeedOrderCollection()
    {

    }
}