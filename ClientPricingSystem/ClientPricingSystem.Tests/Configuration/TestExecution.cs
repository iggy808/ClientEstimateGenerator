using Fixie;
using ClientPricingSystem.Configuration;
using MongoDB.Driver;
using ClientPricingSystem.Tests.Configuration;

class TestExecution : IExecution
{
    public async Task Run(TestSuite testSuite)
    {
        IMongoClient mongoClient = new MongoClient(TestDatabase.DefaultConnectionString);
        IMongoDatabase context = mongoClient.GetDatabase(TestDatabase.DatabaseName);

        foreach (var testClass in testSuite.TestClasses)
        {
            var instance = testClass.Construct(context);
            foreach (var test in testClass.Tests)
            {
                await test.Run(instance);
            }
        }
    }
}